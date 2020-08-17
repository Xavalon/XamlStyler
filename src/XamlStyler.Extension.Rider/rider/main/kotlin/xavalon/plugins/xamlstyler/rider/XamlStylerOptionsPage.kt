package xavalon.plugins.xamlstyler.rider

import com.jetbrains.rider.settings.simple.SimpleOptionsPage

class XamlStylerOptionsPage
    : SimpleOptionsPage("XAML Styler", "XamlStylerOptionsPage") {

    override fun getId(): String = pageId + "Id"
}