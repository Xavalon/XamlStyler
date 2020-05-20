package xavalon.plugins.xamlstyler.dotultimate

import com.intellij.AppTopics
import com.intellij.openapi.application.ApplicationManager
import com.intellij.openapi.command.WriteCommandAction
import com.intellij.openapi.editor.Document
import com.intellij.openapi.fileEditor.FileDocumentManagerListener
import com.intellij.openapi.project.Project
import com.intellij.psi.PsiDocumentManager
import com.jetbrains.rd.util.reactive.adviseOnce
import com.jetbrains.rdclient.util.idea.ProtocolSubscribedProjectComponent
import com.jetbrains.rider.ideaInterop.fileTypes.xaml.XamlLanguage
import com.jetbrains.rider.model.RdXamlStylerFormattingRequest
import com.jetbrains.rider.model.xamlStylerModel
import com.jetbrains.rider.projectView.solution
import com.jetbrains.rider.util.idea.getComponent

class XamlStylerComponent(project: Project)
    : ProtocolSubscribedProjectComponent(project), FileDocumentManagerListener {

    companion object {
        fun getInstance(project: Project) = project.getComponent<XamlStylerComponent>()
    }

    private val model = project.solution.xamlStylerModel
    private val messageBus = ApplicationManager.getApplication().messageBus.connect()

    init {

        // In Rider, documents are saved in the front-end. Since we run XAML Styler in the R# backend,
        // we'll need to subscribe to document sync events and piper the document through the backend
        // before save.
        messageBus.subscribe(AppTopics.FILE_DOCUMENT_SYNC, this)
    }

    override fun beforeDocumentSaving(document: Document) {

        val psiFile = PsiDocumentManager.getInstance(project).getPsiFile(document) ?: return
        if (psiFile.language != XamlLanguage) return

        val filePath = psiFile.virtualFile.path
        val currentDocumentText = document.text

        // Perform reformat on back-end, asynchronously
        model.performReformat.start(componentLifetime, RdXamlStylerFormattingRequest(filePath, currentDocumentText)).result
                .adviseOnce(componentLifetime) { it ->
                    val result = it.unwrap()

                    // Only update if backend actually made modifications
                    if (result.isSuccess && result.hasUpdated) {
                        WriteCommandAction.runWriteCommandAction(project) {
                            document.replaceString(0, currentDocumentText.length, result.formattedText)
                        }
                    }
                }
    }

    override fun dispose() {

        messageBus.disconnect()
        super.dispose()
    }
}