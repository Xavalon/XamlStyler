@file:Suppress("EXPERIMENTAL_API_USAGE","EXPERIMENTAL_UNSIGNED_LITERALS","PackageDirectoryMismatch","UnusedImport","unused","LocalVariableName","CanBeVal","PropertyName","EnumEntryName","ClassName","ObjectPropertyName","UnnecessaryVariable","SpellCheckingInspection")
package com.jetbrains.rider.model

import com.jetbrains.rd.framework.*
import com.jetbrains.rd.framework.base.*
import com.jetbrains.rd.framework.impl.*

import com.jetbrains.rd.util.lifetime.*
import com.jetbrains.rd.util.reactive.*
import com.jetbrains.rd.util.string.*
import com.jetbrains.rd.util.*
import kotlin.reflect.KClass



/**
 * #### Generated from [XamlStylerModel.kt:12]
 */
class XamlStylerModel private constructor(
    private val _performReformat: RdCall<RdXamlStylerFormattingRequest, RdXamlStylerFormattingResult>
) : RdExtBase() {
    //companion
    
    companion object : ISerializersOwner {
        
        override fun registerSerializersCore(serializers: ISerializers)  {
            serializers.register(RdXamlStylerFormattingRequest)
            serializers.register(RdXamlStylerFormattingResult)
        }
        
        
        
        
        const val serializationHash = -3355681540747534071L
        
    }
    override val serializersOwner: ISerializersOwner get() = XamlStylerModel
    override val serializationHash: Long get() = XamlStylerModel.serializationHash
    
    //fields
    val performReformat: IRdCall<RdXamlStylerFormattingRequest, RdXamlStylerFormattingResult> get() = _performReformat
    //methods
    //initializer
    init {
        _performReformat.async = true
    }
    
    init {
        bindableChildren.add("performReformat" to _performReformat)
    }
    
    //secondary constructor
    internal constructor(
    ) : this(
        RdCall<RdXamlStylerFormattingRequest, RdXamlStylerFormattingResult>(RdXamlStylerFormattingRequest, RdXamlStylerFormattingResult)
    )
    
    //equals trait
    //hash code trait
    //pretty print
    override fun print(printer: PrettyPrinter)  {
        printer.println("XamlStylerModel (")
        printer.indent {
            print("performReformat = "); _performReformat.print(printer); println()
        }
        printer.print(")")
    }
    //deepClone
    override fun deepClone(): XamlStylerModel   {
        return XamlStylerModel(
            _performReformat.deepClonePolymorphic()
        )
    }
    //contexts
}
val Solution.xamlStylerModel get() = getOrCreateExtension("xamlStylerModel", ::XamlStylerModel)



/**
 * #### Generated from [XamlStylerModel.kt:14]
 */
data class RdXamlStylerFormattingRequest (
    val filePath: String,
    val documentText: String
) : IPrintable {
    //companion
    
    companion object : IMarshaller<RdXamlStylerFormattingRequest> {
        override val _type: KClass<RdXamlStylerFormattingRequest> = RdXamlStylerFormattingRequest::class
        
        @Suppress("UNCHECKED_CAST")
        override fun read(ctx: SerializationCtx, buffer: AbstractBuffer): RdXamlStylerFormattingRequest  {
            val filePath = buffer.readString()
            val documentText = buffer.readString()
            return RdXamlStylerFormattingRequest(filePath, documentText)
        }
        
        override fun write(ctx: SerializationCtx, buffer: AbstractBuffer, value: RdXamlStylerFormattingRequest)  {
            buffer.writeString(value.filePath)
            buffer.writeString(value.documentText)
        }
        
        
    }
    //fields
    //methods
    //initializer
    //secondary constructor
    //equals trait
    override fun equals(other: Any?): Boolean  {
        if (this === other) return true
        if (other == null || other::class != this::class) return false
        
        other as RdXamlStylerFormattingRequest
        
        if (filePath != other.filePath) return false
        if (documentText != other.documentText) return false
        
        return true
    }
    //hash code trait
    override fun hashCode(): Int  {
        var __r = 0
        __r = __r*31 + filePath.hashCode()
        __r = __r*31 + documentText.hashCode()
        return __r
    }
    //pretty print
    override fun print(printer: PrettyPrinter)  {
        printer.println("RdXamlStylerFormattingRequest (")
        printer.indent {
            print("filePath = "); filePath.print(printer); println()
            print("documentText = "); documentText.print(printer); println()
        }
        printer.print(")")
    }
    //deepClone
    //contexts
}


/**
 * #### Generated from [XamlStylerModel.kt:19]
 */
data class RdXamlStylerFormattingResult (
    val isSuccess: Boolean,
    val hasUpdated: Boolean,
    val formattedText: String
) : IPrintable {
    //companion
    
    companion object : IMarshaller<RdXamlStylerFormattingResult> {
        override val _type: KClass<RdXamlStylerFormattingResult> = RdXamlStylerFormattingResult::class
        
        @Suppress("UNCHECKED_CAST")
        override fun read(ctx: SerializationCtx, buffer: AbstractBuffer): RdXamlStylerFormattingResult  {
            val isSuccess = buffer.readBool()
            val hasUpdated = buffer.readBool()
            val formattedText = buffer.readString()
            return RdXamlStylerFormattingResult(isSuccess, hasUpdated, formattedText)
        }
        
        override fun write(ctx: SerializationCtx, buffer: AbstractBuffer, value: RdXamlStylerFormattingResult)  {
            buffer.writeBool(value.isSuccess)
            buffer.writeBool(value.hasUpdated)
            buffer.writeString(value.formattedText)
        }
        
        
    }
    //fields
    //methods
    //initializer
    //secondary constructor
    //equals trait
    override fun equals(other: Any?): Boolean  {
        if (this === other) return true
        if (other == null || other::class != this::class) return false
        
        other as RdXamlStylerFormattingResult
        
        if (isSuccess != other.isSuccess) return false
        if (hasUpdated != other.hasUpdated) return false
        if (formattedText != other.formattedText) return false
        
        return true
    }
    //hash code trait
    override fun hashCode(): Int  {
        var __r = 0
        __r = __r*31 + isSuccess.hashCode()
        __r = __r*31 + hasUpdated.hashCode()
        __r = __r*31 + formattedText.hashCode()
        return __r
    }
    //pretty print
    override fun print(printer: PrettyPrinter)  {
        printer.println("RdXamlStylerFormattingResult (")
        printer.indent {
            print("isSuccess = "); isSuccess.print(printer); println()
            print("hasUpdated = "); hasUpdated.print(printer); println()
            print("formattedText = "); formattedText.print(printer); println()
        }
        printer.print(")")
    }
    //deepClone
    //contexts
}
