// Reference https://www.jetbrains.org/intellij/sdk/docs/tutorials/build_system/gradle_guide.html
import org.jetbrains.intellij.platform.gradle.Constants
import kotlin.io.path.absolute
import kotlin.io.path.isDirectory
import kotlin.io.path.isRegularFile

plugins {
    id("java")
    alias(libs.plugins.kotlin)
    alias(libs.plugins.intellij.platform)
}

allprojects {
    repositories {
        mavenCentral()
    }
}

repositories {
    intellijPlatform {
        defaultRepositories()
        jetbrainsRuntime()
    }
}

dependencies {
  intellijPlatform {
    rider(libs.versions.rider, false)
    jetbrainsRuntime()
  }
}

val DotnetPluginId: String by project
val DotnetSolution: String by project
val PluginVersion: String by project
val BuildConfiguration: String by project

version = PluginVersion

val riderSdkPath by lazy {
    val path = intellijPlatform.platformPath.resolve("lib/DotNetSdkForRdPlugins").absolute()
    if (!path.isDirectory()) error("$path does not exist or not a directory")

    println("Rider SDK path: $path")
    return@lazy path
}

kotlin {
    jvmToolchain {
        languageVersion = JavaLanguageVersion.of(21)
    }
}

sourceSets {
    main {
        kotlin.srcDir("rider/main/java")
        kotlin.srcDir("rider/main/kotlin")
        resources.srcDir("rider/main/resources")
    }
}

tasks {
    wrapper {
        gradleVersion = "8.10"
        distributionType = Wrapper.DistributionType.ALL
        distributionUrl =
            "https://cache-redirector.jetbrains.com/services.gradle.org/distributions/gradle-${gradleVersion}-all.zip"
    }

    val rdGen = ":protocol:rdgen"

    val compileDotNet by registering(Exec::class) {
        dependsOn(rdGen)
        inputs.property("buildConfiguration", BuildConfiguration)

        executable("dotnet")
        args("msbuild","/t:Restore;Rebuild","${DotnetSolution}","/p:Configuration=${BuildConfiguration}")
        workingDir(rootDir)
    }

    buildPlugin {
        outputs.upToDateWhen { false }
        doLast {
            copy {
                from("${buildDir}/distributions/${rootProject.name}-${version}.zip")
                into("${rootDir}/output")
            }

            val changelogText = file("${rootDir}/CHANGELOG.md").readText()
            val changelogMatches = Regex("(?s)(-.+?)(?=##|$)").findAll(changelogText)
            val changeNotes = changelogMatches.map {
                it.groupValues[1].replace(Regex("(?s)- "), "\u2022 ").replace(Regex("`"), "").replace(Regex(","), "%2C")
            }.take(1).joinToString("")

            exec {
                executable("dotnet")
                args("msbuild","/t:Pack","${DotnetSolution}","/p:Configuration=${BuildConfiguration}","/p:PackageOutputPath=${rootDir}/output","/p:PackageReleaseNotes=${changeNotes}","/p:PackageVersion=${version}")
            }
        }
    }

    intellijPlatform {
        instrumentCode = false
    }

    runIde {
        // Match Rider's default heap size of 1.5Gb (default for runIde is 512Mb)
        maxHeapSize = "1500m"

        // Rider's backend doesn't support dynamic plugins. It might be possible to work with auto-reload of the frontend
        // part of a plugin, but there are dangers about keeping plugins in sync
        autoReload = false

        // gradle-intellij-plugin will download the default version of the JBR for the snapshot. Update if required
        // jbrVersion = "jbr_jcef-11_0_6b765.40" // https://confluence.jetbrains.com/display/JBR/Release+notes
    }

    patchPluginXml {
        val changelogText = file("${rootDir}/CHANGELOG.md").readText()
        val changelogMatches = Regex("(?s)(-.+?)(?=##|$)").findAll(changelogText)

        changeNotes = changelogMatches.map {
            it.groupValues[1].replace(Regex("(?s)\\r?\\n"), "<br />\n")
        }.take(1).joinToString("")
    }

    // buildSearchableOptions.onlyIf { false }

    prepareSandbox {
        dependsOn(compileDotNet)

        val outputFolder = "${rootDir}/dotnet/${DotnetPluginId}/bin/${DotnetPluginId}/${BuildConfiguration}"
        val dllFiles = listOf(
                "$outputFolder/${DotnetPluginId}.dll",
                "$outputFolder/${DotnetPluginId}.pdb",
                "$outputFolder/XamlStyler.dll",
                "$outputFolder/XamlStyler.pdb",
                "$outputFolder/Irony.dll"
        )

        dllFiles.forEach({ f ->
            val file = file(f)
            from(file, { into("${rootProject.name}/dotnet") })
        })

        doLast {
            dllFiles.forEach({ f ->
                val file = file(f)
                if (!file.exists()) error("File ${file} does not exist")
            })
        }
    }
}

val riderModel: Configuration by configurations.creating {
    isCanBeConsumed = true
    isCanBeResolved = false
}

artifacts {
    add(riderModel.name, provider {
        intellijPlatform.platformPath.resolve("lib/rd/rider-model.jar").also {
            check(it.isRegularFile()) {
                "rider-model.jar is not found at $riderModel"
            }
        }
    }) {
        builtBy(Constants.Tasks.INITIALIZE_INTELLIJ_PLATFORM_PLUGIN)
    }
}