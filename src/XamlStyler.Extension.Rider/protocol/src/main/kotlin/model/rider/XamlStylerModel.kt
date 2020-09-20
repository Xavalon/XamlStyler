package model.rider

import com.jetbrains.rd.generator.nova.Ext
import com.jetbrains.rd.generator.nova.PredefinedType.bool
import com.jetbrains.rd.generator.nova.PredefinedType.string
import com.jetbrains.rd.generator.nova.async
import com.jetbrains.rd.generator.nova.call
import com.jetbrains.rd.generator.nova.field
import com.jetbrains.rider.model.nova.ide.SolutionModel

@Suppress("unused")
object XamlStylerModel : Ext(SolutionModel.Solution) {

    private val RdXamlStylerFormattingRequest = structdef {
        field("filePath", string)
        field("documentText", string)
    }

    private val RdXamlStylerFormattingResult = structdef {
        field("isSuccess", bool)
        field("hasUpdated", bool)
        field("formattedText", string)
    }

    init {
        //setting(CSharp50Generator.Namespace, "ReSharperPlugin.XamlStyler.dotUltimate.Rider.Model")
        //setting(Kotlin11Generator.Namespace, "xavalon.plugins.xamlstyler.dotultimate.model")

        call("performReformat", RdXamlStylerFormattingRequest, RdXamlStylerFormattingResult).async
    }
}