# XAML Styler
XAML Styler is a visual studio extension that formats XAML source code based on a set of styling rules. This tool can help you/your team maintain a better XAML coding style as well as a much better XAML readability.

|[Documentation](https://github.com/Xavalon/XamlStyler/wiki)|[Script Integration](https://github.com/Xavalon/XamlStyler/wiki/Script-Integration)|[Release Notes](https://github.com/Xavalon/XamlStyler/releases)|[Contributing](https://github.com/Xavalon/XamlStyler/blob/master/CONTRIBUTING.md)|
|---|---|---|---|

[![Build Status](https://dev.azure.com/xavalon/XAML%20Styler/_apis/build/status/Release?branchName=master)](https://dev.azure.com/xavalon/XAML%20Styler/_build/latest?definitionId=2&branchName=master)

## Downloads
[![VS2022](https://img.shields.io/visual-studio-marketplace/v/TeamXavalon.XAMLStyler2022.svg?label=Visual%20Studio%202022&logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADIAAAAyCAYAAAAeP4ixAAAACXBIWXMAAAsTAAALEwEAmpwYAAADYUlEQVR4nO2aW4hNYRTHv5kxgwkxMi9INA9IJk1SRHIbT0PKC288KHJJLsnLEA88GJoniZRrrg+Ua4SMS+RBiJLwQIaMy4MZZn5aM+vwdWafc759PXNq/nVq9v72Wnv99uxvr/WtvY3pVa88BUwBPgI3gfmmkAWs579uA9NNIQgYAjwGmnR7GN11CagxPVVAP73qnbL2Z9I1oNr0JAHFwGk7Smssm9qBU0CV6QkC9qVHaI25qFV99M0nxEavyHyCpNSQL4glQAfRgXzJB8RM4FemiAKCkOukQ4FtwHvgg1zJkBDVwDeXgIgCBBgIbAZaPGz2A30CQIwA3rkGRFgQoA74msNOktNgHxAVwHM/AREBiCSodZkmo6WXwFgHCPF3x29AhAWxHK12sP8OLMiR8M4ECYiwIMBluYr69y4HH3+ATRlAGoMGRAQgogtAKVAEHHT0dRTob/mR25N8g4hOAiX6O4+bHgLD1c8Y4Cn+1BIHiOiw3udlwFXc9AmYob4GAOcc7V4D4+MCETXq/kHAI9wkmXuZ2hVpTpKKNZOagMoccYQGEe2wFjwvcJckz1K1XQz89DjmhD234gbBGh8JvPVxjiuyAlTbicAb3S95ql7+Y92CMAmA6DETpOL0cZ5XwDi1rdRH/CIvgERBrG7HDx/nkuS50DiKpED02DnZSnIPZb2d8gKCTlBdJGV7InnpuNcETxwEWKrJrkK3V/o9MfAEGJU0SIs1Xgu06f67QLnu3x4ARjqK05ICaZW5oGM1HhNc+kxlOr43AIz4Xx43SLskMt0/Wpe7XjqmpUyx1mhB9C95ivwam3Slja+yMrrkgpwtGZ91WbquZ4gjFMhO3S4H7uOmuVax+MDRxjMgIgI5osWeawkvi6s1Hh2YZ/kEOZvqkgAHHHxIo2JeN0fB6rJY1uz1ftcQGfxU6WM2WRC6uh4bHGxvye2TDcLyOVlrrURBaoHmHHaHUrnDVcBsl7osMhDrqbPW45boLPpMQEnrCPidGEha63SLtjolj9SZkAJWJA4Sl4CtEYM0xxasA0xDhCC78wlSpIk3DEgbsMfvgycOmJL0vldBvgwVyUrRx+vpDm3n9qzX02kfDNyTzzayfDBwEZhkCkl0dSZTugFMNYUmYBbwWdYmqZVpr0wB6y8sXmSTj5yNAwAAAABJRU5ErkJggg==)](https://marketplace.visualstudio.com/items?itemName=TeamXavalon.XAMLStyler2022)

[![VS2019](https://img.shields.io/visual-studio-marketplace/v/TeamXavalon.XAMLStyler.svg?label=Visual%20Studio%202019&logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADIAAAAyCAYAAAAeP4ixAAAACXBIWXMAAAsTAAALEwEAmpwYAAADYUlEQVR4nO2aW4hNYRTHv5kxgwkxMi9INA9IJk1SRHIbT0PKC288KHJJLsnLEA88GJoniZRrrg+Ua4SMS+RBiJLwQIaMy4MZZn5aM+vwdWafc759PXNq/nVq9v72Wnv99uxvr/WtvY3pVa88BUwBPgI3gfmmkAWs579uA9NNIQgYAjwGmnR7GN11CagxPVVAP73qnbL2Z9I1oNr0JAHFwGk7Smssm9qBU0CV6QkC9qVHaI25qFV99M0nxEavyHyCpNSQL4glQAfRgXzJB8RM4FemiAKCkOukQ4FtwHvgg1zJkBDVwDeXgIgCBBgIbAZaPGz2A30CQIwA3rkGRFgQoA74msNOktNgHxAVwHM/AREBiCSodZkmo6WXwFgHCPF3x29AhAWxHK12sP8OLMiR8M4ECYiwIMBluYr69y4HH3+ATRlAGoMGRAQgogtAKVAEHHT0dRTob/mR25N8g4hOAiX6O4+bHgLD1c8Y4Cn+1BIHiOiw3udlwFXc9AmYob4GAOcc7V4D4+MCETXq/kHAI9wkmXuZ2hVpTpKKNZOagMoccYQGEe2wFjwvcJckz1K1XQz89DjmhD234gbBGh8JvPVxjiuyAlTbicAb3S95ql7+Y92CMAmA6DETpOL0cZ5XwDi1rdRH/CIvgERBrG7HDx/nkuS50DiKpED02DnZSnIPZb2d8gKCTlBdJGV7InnpuNcETxwEWKrJrkK3V/o9MfAEGJU0SIs1Xgu06f67QLnu3x4ARjqK05ICaZW5oGM1HhNc+kxlOr43AIz4Xx43SLskMt0/Wpe7XjqmpUyx1mhB9C95ivwam3Slja+yMrrkgpwtGZ91WbquZ4gjFMhO3S4H7uOmuVax+MDRxjMgIgI5osWeawkvi6s1Hh2YZ/kEOZvqkgAHHHxIo2JeN0fB6rJY1uz1ftcQGfxU6WM2WRC6uh4bHGxvye2TDcLyOVlrrURBaoHmHHaHUrnDVcBsl7osMhDrqbPW45boLPpMQEnrCPidGEha63SLtjolj9SZkAJWJA4Sl4CtEYM0xxasA0xDhCC78wlSpIk3DEgbsMfvgycOmJL0vldBvgwVyUrRx+vpDm3n9qzX02kfDNyTzzayfDBwEZhkCkl0dSZTugFMNYUmYBbwWdYmqZVpr0wB6y8sXmSTj5yNAwAAAABJRU5ErkJggg==)](https://marketplace.visualstudio.com/items?itemName=TeamXavalon.XAMLStyler)

[![JetBrains IntelliJ Plugins](https://img.shields.io/jetbrains/plugin/v/14932-xaml-styler?logo=rider&label=JetBrains%20Rider)](https://plugins.jetbrains.com/plugin/14932-xaml-styler)

[![NuGet](https://img.shields.io/nuget/v/XamlStyler.Console.svg?logo=nuget&label=XAML%20Styler%20Console)](https://www.nuget.org/packages/XamlStyler.Console)  
<sub>View [other downloads](https://github.com/Xavalon/XamlStyler/wiki)</sub>

## Getting Started
Right-click with any file and select "Format XAML" to format your XAML source code.

![Context Menu](http://i.imgur.com/gCcNuIS.png)



[![Join the chat at https://gitter.im/Xavalon/XamlStyler](https://badges.gitter.im/Xavalon/XamlStyler.svg)](https://gitter.im/Xavalon/XamlStyler?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) 
