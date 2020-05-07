package xavalon.plugins.xamlstyler.dotultimate

import com.jetbrains.rider.settings.simple.SimpleOptionsPage

class XamlStylerOptionsPage
    : SimpleOptionsPage("XAML Styler", "XamlStylerOptionsPage") {

    override fun getId(): String = pageId + "Id"
}