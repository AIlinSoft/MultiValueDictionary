using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace AIlins.Collections.Generic
{
    internal enum ExceptionArgument
    {
        obj,
        dictionary,
        dictionaryCreationThreshold,
        array,
        info,
        key,
        collection,
        list,
        match,
        converter,
        queue,
        stack,
        capacity,
        index,
        startIndex,
        value,
        count,
        arrayIndex,
        name,
        mode,
        item,
        options,
        view,
        sourceBytesToCopy
    }
    internal static class ThrowHelper
    {
        internal static string GetResourceString(string key)
        {
            return key;
        }
        internal static string GetResourceString(string key, params object[] values)
        {
            string resourceString = GetResourceString(key);
            return string.Format(CultureInfo.CurrentCulture, resourceString, values);
        }
        internal static void ThrowArgumentOutOfRangeException()
        {
            ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index);
        }

        internal static void ThrowWrongKeyTypeArgumentException(object key, Type targetType)
        {
            throw new ArgumentException(GetResourceString("Arg_WrongType", key, targetType), "key");
        }

        internal static void ThrowWrongValueTypeArgumentException(object value, Type targetType)
        {
            throw new ArgumentException(GetResourceString("Arg_WrongType", value, targetType), "value");
        }

        internal static void ThrowKeyNotFoundException()
        {
            throw new KeyNotFoundException();
        }

        internal static void ThrowArgumentException(ExceptionResource resource)
        {
            throw new ArgumentException(GetResourceString(GetResourceName(resource)));
        }

        internal static void ThrowArgumentException(ExceptionResource resource, ExceptionArgument argument)
        {
            throw new ArgumentException(GetResourceString(GetResourceName(resource)), GetArgumentName(argument));
        }

        internal static void ThrowArgumentNullException(ExceptionArgument argument)
        {
            throw new ArgumentNullException(GetArgumentName(argument));
        }

        internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument)
        {
            throw new ArgumentOutOfRangeException(GetArgumentName(argument));
        }

        internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
        {
            throw new ArgumentOutOfRangeException(GetArgumentName(argument), GetResourceString(GetResourceName(resource)));
        }

        internal static void ThrowInvalidOperationException(ExceptionResource resource)
        {
            throw new InvalidOperationException(GetResourceString(GetResourceName(resource)));
        }

        internal static void ThrowSerializationException(ExceptionResource resource)
        {
            throw new SerializationException(GetResourceString(GetResourceName(resource)));
        }

        internal static void ThrowSecurityException(ExceptionResource resource)
        {
            throw new SecurityException(GetResourceString(GetResourceName(resource)));
        }

        internal static void ThrowNotSupportedException(ExceptionResource resource)
        {
            throw new NotSupportedException(GetResourceString(GetResourceName(resource)));
        }

        internal static void ThrowUnauthorizedAccessException(ExceptionResource resource)
        {
            throw new UnauthorizedAccessException(GetResourceString(GetResourceName(resource)));
        }

        internal static void ThrowObjectDisposedException(string objectName, ExceptionResource resource)
        {
            throw new ObjectDisposedException(objectName, GetResourceString(GetResourceName(resource)));
        }

        internal static void IfNullAndNullsAreIllegalThenThrow<T>(object value, ExceptionArgument argName)
        {
            if (value == null && default(T) != null)
            {
                ThrowArgumentNullException(argName);
            }
        }

        internal static string GetArgumentName(ExceptionArgument argument)
        {
            string text = null;
            switch (argument)
            {
                case ExceptionArgument.array:
                    return "array";
                case ExceptionArgument.arrayIndex:
                    return "arrayIndex";
                case ExceptionArgument.capacity:
                    return "capacity";
                case ExceptionArgument.collection:
                    return "collection";
                case ExceptionArgument.list:
                    return "list";
                case ExceptionArgument.converter:
                    return "converter";
                case ExceptionArgument.count:
                    return "count";
                case ExceptionArgument.dictionary:
                    return "dictionary";
                case ExceptionArgument.dictionaryCreationThreshold:
                    return "dictionaryCreationThreshold";
                case ExceptionArgument.index:
                    return "index";
                case ExceptionArgument.info:
                    return "info";
                case ExceptionArgument.key:
                    return "key";
                case ExceptionArgument.match:
                    return "match";
                case ExceptionArgument.obj:
                    return "obj";
                case ExceptionArgument.queue:
                    return "queue";
                case ExceptionArgument.stack:
                    return "stack";
                case ExceptionArgument.startIndex:
                    return "startIndex";
                case ExceptionArgument.value:
                    return "value";
                case ExceptionArgument.name:
                    return "name";
                case ExceptionArgument.mode:
                    return "mode";
                case ExceptionArgument.item:
                    return "item";
                case ExceptionArgument.options:
                    return "options";
                case ExceptionArgument.view:
                    return "view";
                case ExceptionArgument.sourceBytesToCopy:
                    return "sourceBytesToCopy";
                default:
                    return string.Empty;
            }
        }

        internal static string GetResourceName(ExceptionResource resource)
        {
            string text = null;
            switch (resource)
            {
                case ExceptionResource.Argument_ImplementIComparable:
                    return "Argument_ImplementIComparable";
                case ExceptionResource.Argument_AddingDuplicate:
                    return "Argument_AddingDuplicate";
                case ExceptionResource.ArgumentOutOfRange_BiggerThanCollection:
                    return "ArgumentOutOfRange_BiggerThanCollection";
                case ExceptionResource.ArgumentOutOfRange_Count:
                    return "ArgumentOutOfRange_Count";
                case ExceptionResource.ArgumentOutOfRange_Index:
                    return "ArgumentOutOfRange_Index";
                case ExceptionResource.ArgumentOutOfRange_InvalidThreshold:
                    return "ArgumentOutOfRange_InvalidThreshold";
                case ExceptionResource.ArgumentOutOfRange_ListInsert:
                    return "ArgumentOutOfRange_ListInsert";
                case ExceptionResource.ArgumentOutOfRange_NeedNonNegNum:
                    return "ArgumentOutOfRange_NeedNonNegNum";
                case ExceptionResource.ArgumentOutOfRange_SmallCapacity:
                    return "ArgumentOutOfRange_SmallCapacity";
                case ExceptionResource.Arg_ArrayPlusOffTooSmall:
                    return "Arg_ArrayPlusOffTooSmall";
                case ExceptionResource.Arg_RankMultiDimNotSupported:
                    return "Arg_RankMultiDimNotSupported";
                case ExceptionResource.Arg_NonZeroLowerBound:
                    return "Arg_NonZeroLowerBound";
                case ExceptionResource.Argument_InvalidArrayType:
                    return "Argument_InvalidArrayType";
                case ExceptionResource.Argument_InvalidOffLen:
                    return "Argument_InvalidOffLen";
                case ExceptionResource.Argument_ItemNotExist:
                    return "Argument_ItemNotExist";
                case ExceptionResource.InvalidOperation_CannotRemoveFromStackOrQueue:
                    return "InvalidOperation_CannotRemoveFromStackOrQueue";
                case ExceptionResource.InvalidOperation_EmptyQueue:
                    return "InvalidOperation_EmptyQueue";
                case ExceptionResource.InvalidOperation_EnumOpCantHappen:
                    return "InvalidOperation_EnumOpCantHappen";
                case ExceptionResource.InvalidOperation_EnumFailedVersion:
                    return "InvalidOperation_EnumFailedVersion";
                case ExceptionResource.InvalidOperation_EmptyStack:
                    return "InvalidOperation_EmptyStack";
                case ExceptionResource.InvalidOperation_EnumNotStarted:
                    return "InvalidOperation_EnumNotStarted";
                case ExceptionResource.InvalidOperation_EnumEnded:
                    return "InvalidOperation_EnumEnded";
                case ExceptionResource.NotSupported_KeyCollectionSet:
                    return "NotSupported_KeyCollectionSet";
                case ExceptionResource.NotSupported_ReadOnlyCollection:
                    return "NotSupported_ReadOnlyCollection";
                case ExceptionResource.NotSupported_ValueCollectionSet:
                    return "NotSupported_ValueCollectionSet";
                case ExceptionResource.NotSupported_SortedListNestedWrite:
                    return "NotSupported_SortedListNestedWrite";
                case ExceptionResource.Serialization_InvalidOnDeser:
                    return "Serialization_InvalidOnDeser";
                case ExceptionResource.Serialization_MissingKeys:
                    return "Serialization_MissingKeys";
                case ExceptionResource.Serialization_NullKey:
                    return "Serialization_NullKey";
                case ExceptionResource.Argument_InvalidType:
                    return "Argument_InvalidType";
                case ExceptionResource.Argument_InvalidArgumentForComparison:
                    return "Argument_InvalidArgumentForComparison";
                case ExceptionResource.InvalidOperation_NoValue:
                    return "InvalidOperation_NoValue";
                case ExceptionResource.InvalidOperation_RegRemoveSubKey:
                    return "InvalidOperation_RegRemoveSubKey";
                case ExceptionResource.Arg_RegSubKeyAbsent:
                    return "Arg_RegSubKeyAbsent";
                case ExceptionResource.Arg_RegSubKeyValueAbsent:
                    return "Arg_RegSubKeyValueAbsent";
                case ExceptionResource.Arg_RegKeyDelHive:
                    return "Arg_RegKeyDelHive";
                case ExceptionResource.Security_RegistryPermission:
                    return "Security_RegistryPermission";
                case ExceptionResource.Arg_RegSetStrArrNull:
                    return "Arg_RegSetStrArrNull";
                case ExceptionResource.Arg_RegSetMismatchedKind:
                    return "Arg_RegSetMismatchedKind";
                case ExceptionResource.UnauthorizedAccess_RegistryNoWrite:
                    return "UnauthorizedAccess_RegistryNoWrite";
                case ExceptionResource.ObjectDisposed_RegKeyClosed:
                    return "ObjectDisposed_RegKeyClosed";
                case ExceptionResource.Arg_RegKeyStrLenBug:
                    return "Arg_RegKeyStrLenBug";
                case ExceptionResource.Argument_InvalidRegistryKeyPermissionCheck:
                    return "Argument_InvalidRegistryKeyPermissionCheck";
                case ExceptionResource.NotSupported_InComparableType:
                    return "NotSupported_InComparableType";
                case ExceptionResource.Argument_InvalidRegistryOptionsCheck:
                    return "Argument_InvalidRegistryOptionsCheck";
                case ExceptionResource.Argument_InvalidRegistryViewCheck:
                    return "Argument_InvalidRegistryViewCheck";
                default:
                    return string.Empty;
            }
        }
    }

}
