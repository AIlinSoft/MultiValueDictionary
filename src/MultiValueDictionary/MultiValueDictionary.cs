using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Threading;
namespace AIlins.Collections.Generic
{
    /// <summary>
    ///         Представляет коллекцию ключей и значений.
    ///
    ///         Исходный код .NET Framework для этого типа см. в указанном источнике.
    ///       </summary>
    /// <typeparam name="TKey">
    ///           Тип ключей в словаре.
    ///         </typeparam>
    /// <typeparam name="TValue">
    ///           Тип значений в словаре.
    ///         </typeparam>
    [Serializable]
    [DebuggerDisplay("Count = {Count}")]
    [ComVisible(false)]

    public class MultiValueDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IDictionary, ICollection, IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>, ISerializable, IDeserializationCallback
    {
        private struct Entry
        {
            public int hashCode;

            public int next;

            public TKey key;

            public TValue value;
        }

        /// <summary>
        ///         Выполняет перечисление элементов коллекции <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///       </summary>
        [Serializable]

        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IEnumerator, IDictionaryEnumerator
        {
            private MultiValueDictionary<TKey, TValue> dictionary;

            private int version;

            private int index;

            private KeyValuePair<TKey, TValue> current;

            private int getEnumeratorRetType;

            internal const int DictEntry = 1;

            internal const int KeyValuePair = 2;

            /// <summary>
            ///         Возвращает элемент, расположенный в текущей позиции перечислителя.
            ///       </summary>
            /// <returns>
            ///         Элемент коллекции <see cref="T:System.Collections.Generic.Dictionary`2" />, находящийся в текущей позиции перечислителя.
            ///       </returns>

            public KeyValuePair<TKey, TValue> Current
            {

                get
                {
                    return current;
                }
            }

            /// <summary>
            ///         Возвращает элемент, расположенный в текущей позиции перечислителя.
            ///       </summary>
            /// <returns>
            ///         Элемент в коллекции, соответствующий текущей позиции перечислителя, как <see cref="T:System.Object" />.
            ///       </returns>
            /// <exception cref="T:System.InvalidOperationException">
            ///             Перечислитель располагается перед первым элементом коллекции или после последнего элемента.
            ///           </exception>

            object IEnumerator.Current
            {

                get
                {
                    if (index == 0 || index == dictionary.count + 1)
                    {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    if (getEnumeratorRetType == 1)
                    {
                        return new DictionaryEntry(current.Key, current.Value);
                    }
                    return new KeyValuePair<TKey, TValue>(current.Key, current.Value);
                }
            }

            /// <summary>
            ///         Возвращает элемент, расположенный в текущей позиции перечислителя.
            ///       </summary>
            /// <returns>
            ///         Элемент в словаре соответствующий текущей позиции перечислителя, как <see cref="T:System.Collections.DictionaryEntry" />.
            ///       </returns>
            /// <exception cref="T:System.InvalidOperationException">
            ///             Перечислитель располагается перед первым элементом коллекции или после последнего элемента.
            ///           </exception>

            DictionaryEntry IDictionaryEnumerator.Entry
            {

                get
                {
                    if (index == 0 || index == dictionary.count + 1)
                    {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    return new DictionaryEntry(current.Key, current.Value);
                }
            }

            /// <summary>
            ///         Возвращает ключ элемента в текущей позиции перечислителя.
            ///       </summary>
            /// <returns>
            ///         Ключ элемента словаря, расположенного в текущей позиции перечислителя.
            ///       </returns>
            /// <exception cref="T:System.InvalidOperationException">
            ///             Перечислитель располагается перед первым элементом коллекции или после последнего элемента.
            ///           </exception>

            object IDictionaryEnumerator.Key
            {

                get
                {
                    if (index == 0 || index == dictionary.count + 1)
                    {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    return current.Key;
                }
            }

            /// <summary>
            ///         Возвращает значение элемента в текущей позиции перечислителя.
            ///       </summary>
            /// <returns>
            ///         Значение элемента словаря, расположенного в текущей позиции перечислителя.
            ///       </returns>
            /// <exception cref="T:System.InvalidOperationException">
            ///             Перечислитель располагается перед первым элементом коллекции или после последнего элемента.
            ///           </exception>

            object IDictionaryEnumerator.Value
            {

                get
                {
                    if (index == 0 || index == dictionary.count + 1)
                    {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    return current.Value;
                }
            }

            internal Enumerator(MultiValueDictionary<TKey, TValue> dictionary, int getEnumeratorRetType)
            {
                this.dictionary = dictionary;
                version = dictionary.version;
                index = 0;
                this.getEnumeratorRetType = getEnumeratorRetType;
                current = default(KeyValuePair<TKey, TValue>);
            }

            /// <summary>
            ///         Перемещает перечислитель к следующему элементу коллекции <see cref="T:System.Collections.Generic.Dictionary`2" />.
            ///       </summary>
            /// <returns>
            ///         Значение <see langword="true" />, если перечислитель был успешно перемещен к следующему элементу; значение <see langword="false" />, если перечислитель достиг конца коллекции.
            ///       </returns>
            /// <exception cref="T:System.InvalidOperationException">
            ///             Коллекция была изменена после создания перечислителя.
            ///           </exception>

            public bool MoveNext()
            {
                if (version != dictionary.version)
                {
                    ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                }
                while ((uint)index < (uint)dictionary.count)
                {
                    if (dictionary.entries[index].hashCode >= 0)
                    {
                        current = new KeyValuePair<TKey, TValue>(dictionary.entries[index].key, dictionary.entries[index].value);
                        index++;
                        return true;
                    }
                    index++;
                }
                index = dictionary.count + 1;
                current = default(KeyValuePair<TKey, TValue>);
                return false;
            }

            /// <summary>
            ///         Освобождает все ресурсы, занятые модулем <see cref="T:System.Collections.Generic.Dictionary`2.Enumerator" />.
            ///       </summary>

            public void Dispose()
            {
            }

            /// <summary>
            ///         Устанавливает перечислитель в его начальное положение, т. е. перед первым элементом коллекции.
            ///       </summary>
            /// <exception cref="T:System.InvalidOperationException">
            ///             Коллекция была изменена после создания перечислителя.
            ///           </exception>

            void IEnumerator.Reset()
            {
                if (version != dictionary.version)
                {
                    ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                }
                index = 0;
                current = default(KeyValuePair<TKey, TValue>);
            }
        }

        /// <summary>
        ///         Представляет коллекцию ключей в <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///          Этот класс не наследуется.
        ///       </summary>
        [Serializable]
        [DebuggerDisplay("Count = {Count}")]

        public sealed class KeyCollection : ICollection<TKey>, IEnumerable<TKey>, IEnumerable, ICollection, IReadOnlyCollection<TKey>
        {
            /// <summary>
            ///         Выполняет перечисление элементов коллекции <see cref="T:System.Collections.Generic.Dictionary`2.KeyCollection" />.
            ///       </summary>
            [Serializable]

            public struct Enumerator : IEnumerator<TKey>, IDisposable, IEnumerator
            {
                private MultiValueDictionary<TKey, TValue> dictionary;

                private int index;

                private int version;

                private TKey currentKey;

                /// <summary>
                ///         Возвращает элемент, расположенный в текущей позиции перечислителя.
                ///       </summary>
                /// <returns>
                ///         Элемент коллекции <see cref="T:System.Collections.Generic.Dictionary`2.KeyCollection" />, находящийся в текущей позиции перечислителя.
                ///       </returns>

                public TKey Current
                {

                    get
                    {
                        return currentKey;
                    }
                }

                /// <summary>
                ///         Возвращает элемент, расположенный в текущей позиции перечислителя.
                ///       </summary>
                /// <returns>
                ///         Элемент коллекции, соответствующий текущей позиции перечислителя.
                ///       </returns>
                /// <exception cref="T:System.InvalidOperationException">
                ///             Перечислитель располагается перед первым элементом коллекции или после последнего элемента.
                ///           </exception>

                object IEnumerator.Current
                {

                    get
                    {
                        if (index == 0 || index == dictionary.count + 1)
                        {
                            ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                        }
                        return currentKey;
                    }
                }

                internal Enumerator(MultiValueDictionary<TKey, TValue> dictionary)
                {
                    this.dictionary = dictionary;
                    version = dictionary.version;
                    index = 0;
                    currentKey = default(TKey);
                }

                /// <summary>
                ///         Освобождает все ресурсы, занятые модулем <see cref="T:System.Collections.Generic.Dictionary`2.KeyCollection.Enumerator" />.
                ///       </summary>

                public void Dispose()
                {
                }

                /// <summary>
                ///         Перемещает перечислитель к следующему элементу коллекции <see cref="T:System.Collections.Generic.Dictionary`2.KeyCollection" />.
                ///       </summary>
                /// <returns>
                ///         Значение <see langword="true" />, если перечислитель был успешно перемещен к следующему элементу; значение <see langword="false" />, если перечислитель достиг конца коллекции.
                ///       </returns>
                /// <exception cref="T:System.InvalidOperationException">
                ///             Коллекция была изменена после создания перечислителя.
                ///           </exception>

                public bool MoveNext()
                {
                    if (version != dictionary.version)
                    {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                    }
                    while ((uint)index < (uint)dictionary.count)
                    {
                        if (dictionary.entries[index].hashCode >= 0)
                        {
                            currentKey = dictionary.entries[index].key;
                            index++;
                            return true;
                        }
                        index++;
                    }
                    index = dictionary.count + 1;
                    currentKey = default(TKey);
                    return false;
                }

                /// <summary>
                ///         Устанавливает перечислитель в его начальное положение, т. е. перед первым элементом коллекции.
                ///       </summary>
                /// <exception cref="T:System.InvalidOperationException">
                ///             Коллекция была изменена после создания перечислителя.
                ///           </exception>

                void IEnumerator.Reset()
                {
                    if (version != dictionary.version)
                    {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                    }
                    index = 0;
                    currentKey = default(TKey);
                }
            }

            private MultiValueDictionary<TKey, TValue> dictionary;

            /// <summary>
            ///         Получает число элементов, содержащихся в интерфейсе <see cref="T:System.Collections.Generic.Dictionary`2.KeyCollection" />.
            ///       </summary>
            /// <returns>
            ///         Число элементов, содержащихся в коллекции <see cref="T:System.Collections.Generic.Dictionary`2.KeyCollection" />.
            ///
            ///         Получение значения данного свойства является операцией порядка сложности O(1).
            ///       </returns>

            public int Count
            {

                get
                {
                    return dictionary.Count;
                }
            }


            bool ICollection<TKey>.IsReadOnly
            {

                get
                {
                    return true;
                }
            }

            /// <summary>
            ///         Возвращает значение, показывающее, является ли доступ к коллекции <see cref="T:System.Collections.ICollection" /> синхронизированным (потокобезопасным).
            ///       </summary>
            /// <returns>
            ///         <see langword="true" />, если доступ к классу <see cref="T:System.Collections.ICollection" /> является синхронизированным (потокобезопасным); в противном случае — <see langword="false" />.
            ///           В используемой по умолчанию реализации <see cref="T:System.Collections.Generic.Dictionary`2.KeyCollection" /> это свойство всегда возвращает значение <see langword="false" />.
            ///       </returns>

            bool ICollection.IsSynchronized
            {

                get
                {
                    return false;
                }
            }

            /// <summary>
            ///         Получает объект, с помощью которого можно синхронизировать доступ к коллекции <see cref="T:System.Collections.ICollection" />.
            ///       </summary>
            /// <returns>
            ///         Объект, который может использоваться для синхронизации доступа к <see cref="T:System.Collections.ICollection" />.
            ///           В используемой по умолчанию реализации <see cref="T:System.Collections.Generic.Dictionary`2.KeyCollection" /> это свойство всегда возвращает текущий экземпляр.
            ///       </returns>

            object ICollection.SyncRoot
            {

                get
                {
                    return ((ICollection)dictionary).SyncRoot;
                }
            }

            /// <summary>
            ///         Инициализирует новый экземпляр <see cref="T:System.Collections.Generic.Dictionary`2.KeyCollection" /> класс, который отражает ключи в указанном <see cref="T:System.Collections.Generic.Dictionary`2" />.
            ///       </summary>
            /// <param name="dictionary">
            ///           <see cref="T:System.Collections.Generic.Dictionary`2" /> Ключи которых отражаются в новом <see cref="T:System.Collections.Generic.Dictionary`2.KeyCollection" />.
            ///         </param>
            /// <exception cref="T:System.ArgumentNullException">
            ///             Свойство <paramref name="dictionary" /> имеет значение <see langword="null" />.
            ///           </exception>

            public KeyCollection(MultiValueDictionary<TKey, TValue> dictionary)
            {
                if (dictionary == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);
                }
                this.dictionary = dictionary;
            }

            /// <summary>
            ///         Возвращает перечислитель, осуществляющий перебор элементов списка <see cref="T:System.Collections.Generic.Dictionary`2.KeyCollection" />.
            ///       </summary>
            /// <returns>
            ///         Новый объект <see cref="T:System.Collections.Generic.Dictionary`2.KeyCollection.Enumerator" /> для <see cref="T:System.Collections.Generic.Dictionary`2.KeyCollection" />.
            ///       </returns>

            public Enumerator GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            /// <summary>
            ///         Копирует элементы коллекции <see cref="T:System.Collections.Generic.Dictionary`2.KeyCollection" /> в существующий одномерный массив <see cref="T:System.Array" />, начиная с указанного значения индекса массива.
            ///       </summary>
            /// <param name="array">
            ///           Одномерный массив <see cref="T:System.Array" />, в который копируются элементы из интерфейса <see cref="T:System.Collections.Generic.Dictionary`2.KeyCollection" />.
            ///            Массив <see cref="T:System.Array" /> должен иметь индексацию, начинающуюся с нуля.
            ///         </param>
            /// <param name="index">
            ///           Отсчитываемый от нуля индекс в массиве <paramref name="array" />, указывающий начало копирования.
            ///         </param>
            /// <exception cref="T:System.ArgumentNullException">
            ///             Свойство <paramref name="array" /> имеет значение <see langword="null" />.
            ///           </exception>
            /// <exception cref="T:System.ArgumentOutOfRangeException">
            ///             Значение параметра <paramref name="index" /> меньше нуля.
            ///           </exception>
            /// <exception cref="T:System.ArgumentException">
            ///             Количество элементов в исходной коллекции <see cref="T:System.Collections.Generic.Dictionary`2.KeyCollection" /> больше, чем свободное пространство от <paramref name="index" /> до конца массива назначения <paramref name="array" />.
            ///           </exception>

            public void CopyTo(TKey[] array, int index)
            {
                if (array == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                }
                if (index < 0 || index > array.Length)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
                }
                if (array.Length - index < dictionary.Count)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
                }
                int count = dictionary.count;
                Entry[] entries = dictionary.entries;
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].hashCode >= 0)
                    {
                        array[index++] = entries[i].key;
                    }
                }
            }


            void ICollection<TKey>.Add(TKey item)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);
            }


            void ICollection<TKey>.Clear()
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);
            }


            bool ICollection<TKey>.Contains(TKey item)
            {
                return dictionary.ContainsKey(item);
            }


            bool ICollection<TKey>.Remove(TKey item)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);
                return false;
            }


            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            /// <summary>
            ///         Возвращает перечислитель, который осуществляет итерацию по коллекции.
            ///       </summary>
            /// <returns>
            ///         Объект <see cref="T:System.Collections.IEnumerator" />, который может использоваться для итерации элементов коллекции.
            ///       </returns>

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            /// <summary>
            ///         Копирует элементы коллекции <see cref="T:System.Collections.ICollection" /> в массив <see cref="T:System.Array" />, начиная с указанного индекса массива <see cref="T:System.Array" />.
            ///       </summary>
            /// <param name="array">
            ///           Одномерный массив <see cref="T:System.Array" />, в который копируются элементы из интерфейса <see cref="T:System.Collections.ICollection" />.
            ///            Массив <see cref="T:System.Array" /> должен иметь индексацию, начинающуюся с нуля.
            ///         </param>
            /// <param name="index">
            ///           Отсчитываемый от нуля индекс в массиве <paramref name="array" />, указывающий начало копирования.
            ///         </param>
            /// <exception cref="T:System.ArgumentNullException">
            ///             Свойство <paramref name="array" /> имеет значение <see langword="null" />.
            ///           </exception>
            /// <exception cref="T:System.ArgumentOutOfRangeException">
            ///             Значение параметра <paramref name="index" /> меньше нуля.
            ///           </exception>
            /// <exception cref="T:System.ArgumentException">
            ///             Массив <paramref name="array" /> является многомерным.
            ///
            ///             -или-
            ///
            ///             Массив <paramref name="array" /> не имеет индексации, начинающейся с нуля.
            ///
            ///             -или-
            ///
            ///             Количество элементов в исходной коллекции <see cref="T:System.Collections.ICollection" /> больше, чем свободное пространство от <paramref name="index" /> до конца массива назначения <paramref name="array" />.
            ///
            ///             -или-
            ///
            ///             Тип источника <see cref="T:System.Collections.ICollection" /> не может быть автоматически приведен к типу массива назначения <paramref name="array" />.
            ///           </exception>

            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                }
                if (array.Rank != 1)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
                }
                if (array.GetLowerBound(0) != 0)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
                }
                if (index < 0 || index > array.Length)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
                }
                if (array.Length - index < dictionary.Count)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
                }
                TKey[] array2 = array as TKey[];
                if (array2 != null)
                {
                    CopyTo(array2, index);
                }
                else
                {
                    object[] array3 = array as object[];
                    if (array3 == null)
                    {
                        ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
                    }
                    int count = dictionary.count;
                    Entry[] entries = dictionary.entries;
                    try
                    {
                        for (int i = 0; i < count; i++)
                        {
                            if (entries[i].hashCode >= 0)
                            {
                                array3[index++] = entries[i].key;
                            }
                        }
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
                    }
                }
            }
        }

        /// <summary>
        ///         Представляет коллекцию значений в <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///          Этот класс не наследуется.
        ///       </summary>
        [Serializable]
        [DebuggerDisplay("Count = {Count}")]

        public sealed class ValueCollection : ICollection<TValue>, IEnumerable<TValue>, IEnumerable, ICollection, IReadOnlyCollection<TValue>
        {
            /// <summary>
            ///         Выполняет перечисление элементов коллекции <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" />.
            ///       </summary>
            [Serializable]

            public struct Enumerator : IEnumerator<TValue>, IDisposable, IEnumerator
            {
                private MultiValueDictionary<TKey, TValue> dictionary;

                private int index;

                private int version;

                private TValue currentValue;

                /// <summary>
                ///         Возвращает элемент, расположенный в текущей позиции перечислителя.
                ///       </summary>
                /// <returns>
                ///         Элемент коллекции <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" />, находящийся в текущей позиции перечислителя.
                ///       </returns>

                public TValue Current
                {

                    get
                    {
                        return currentValue;
                    }
                }

                /// <summary>
                ///         Возвращает элемент, расположенный в текущей позиции перечислителя.
                ///       </summary>
                /// <returns>
                ///         Элемент коллекции, соответствующий текущей позиции перечислителя.
                ///       </returns>
                /// <exception cref="T:System.InvalidOperationException">
                ///             Перечислитель располагается перед первым элементом коллекции или после последнего элемента.
                ///           </exception>

                object IEnumerator.Current
                {

                    get
                    {
                        if (index == 0 || index == dictionary.count + 1)
                        {
                            ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                        }
                        return currentValue;
                    }
                }

                internal Enumerator(MultiValueDictionary<TKey, TValue> dictionary)
                {
                    this.dictionary = dictionary;
                    version = dictionary.version;
                    index = 0;
                    currentValue = default(TValue);
                }

                /// <summary>
                ///         Освобождает все ресурсы, занятые модулем <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection.Enumerator" />.
                ///       </summary>

                public void Dispose()
                {
                }

                /// <summary>
                ///         Перемещает перечислитель к следующему элементу коллекции <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" />.
                ///       </summary>
                /// <returns>
                ///         Значение <see langword="true" />, если перечислитель был успешно перемещен к следующему элементу; значение <see langword="false" />, если перечислитель достиг конца коллекции.
                ///       </returns>
                /// <exception cref="T:System.InvalidOperationException">
                ///             Коллекция была изменена после создания перечислителя.
                ///           </exception>

                public bool MoveNext()
                {
                    if (version != dictionary.version)
                    {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                    }
                    while ((uint)index < (uint)dictionary.count)
                    {
                        if (dictionary.entries[index].hashCode >= 0)
                        {
                            currentValue = dictionary.entries[index].value;
                            index++;
                            return true;
                        }
                        index++;
                    }
                    index = dictionary.count + 1;
                    currentValue = default(TValue);
                    return false;
                }

                /// <summary>
                ///         Устанавливает перечислитель в его начальное положение, т. е. перед первым элементом коллекции.
                ///       </summary>
                /// <exception cref="T:System.InvalidOperationException">
                ///             Коллекция была изменена после создания перечислителя.
                ///           </exception>

                void IEnumerator.Reset()
                {
                    if (version != dictionary.version)
                    {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                    }
                    index = 0;
                    currentValue = default(TValue);
                }
            }

            private MultiValueDictionary<TKey, TValue> dictionary;

            /// <summary>
            ///         Получает число элементов, содержащихся в интерфейсе <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" />.
            ///       </summary>
            /// <returns>
            ///         Число элементов, содержащихся в коллекции <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" />.
            ///       </returns>

            public int Count
            {

                get
                {
                    return dictionary.Count;
                }
            }


            bool ICollection<TValue>.IsReadOnly
            {

                get
                {
                    return true;
                }
            }

            /// <summary>
            ///         Возвращает значение, показывающее, является ли доступ к коллекции <see cref="T:System.Collections.ICollection" /> синхронизированным (потокобезопасным).
            ///       </summary>
            /// <returns>
            ///         <see langword="true" />, если доступ к классу <see cref="T:System.Collections.ICollection" /> является синхронизированным (потокобезопасным); в противном случае — <see langword="false" />.
            ///           В используемой по умолчанию реализации <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" /> это свойство всегда возвращает значение <see langword="false" />.
            ///       </returns>

            bool ICollection.IsSynchronized
            {

                get
                {
                    return false;
                }
            }

            /// <summary>
            ///         Получает объект, с помощью которого можно синхронизировать доступ к коллекции <see cref="T:System.Collections.ICollection" />.
            ///       </summary>
            /// <returns>
            ///         Объект, который может использоваться для синхронизации доступа к <see cref="T:System.Collections.ICollection" />.
            ///           В используемой по умолчанию реализации <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" /> это свойство всегда возвращает текущий экземпляр.
            ///       </returns>

            object ICollection.SyncRoot
            {

                get
                {
                    return ((ICollection)dictionary).SyncRoot;
                }
            }

            /// <summary>
            ///         Инициализирует новый экземпляр <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" /> класс, который отражает значения в указанном <see cref="T:System.Collections.Generic.Dictionary`2" />.
            ///       </summary>
            /// <param name="dictionary">
            ///           <see cref="T:System.Collections.Generic.Dictionary`2" /> Значения которых отражаются в новом <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" />.
            ///         </param>
            /// <exception cref="T:System.ArgumentNullException">
            ///             Свойство <paramref name="dictionary" /> имеет значение <see langword="null" />.
            ///           </exception>

            public ValueCollection(MultiValueDictionary<TKey, TValue> dictionary)
            {
                if (dictionary == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);
                }
                this.dictionary = dictionary;
            }

            /// <summary>
            ///         Возвращает перечислитель, осуществляющий перебор элементов списка <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" />.
            ///       </summary>
            /// <returns>
            ///         Новый объект <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection.Enumerator" /> для <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" />.
            ///       </returns>

            public Enumerator GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            /// <summary>
            ///         Копирует элементы коллекции <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" /> в существующий одномерный массив <see cref="T:System.Array" />, начиная с указанного значения индекса массива.
            ///       </summary>
            /// <param name="array">
            ///           Одномерный массив <see cref="T:System.Array" />, в который копируются элементы из интерфейса <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" />.
            ///            Массив <see cref="T:System.Array" /> должен иметь индексацию, начинающуюся с нуля.
            ///         </param>
            /// <param name="index">
            ///           Отсчитываемый от нуля индекс в массиве <paramref name="array" />, указывающий начало копирования.
            ///         </param>
            /// <exception cref="T:System.ArgumentNullException">
            ///             Свойство <paramref name="array" /> имеет значение <see langword="null" />.
            ///           </exception>
            /// <exception cref="T:System.ArgumentOutOfRangeException">
            ///             Значение параметра <paramref name="index" /> меньше нуля.
            ///           </exception>
            /// <exception cref="T:System.ArgumentException">
            ///             Число элементов в исходном массиве <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" /> больше доступного места от положения, заданного значением параметра <paramref name="index" />, до конца массива назначения <paramref name="array" />.
            ///           </exception>

            public void CopyTo(TValue[] array, int index)
            {
                if (array == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                }
                if (index < 0 || index > array.Length)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
                }
                if (array.Length - index < dictionary.Count)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
                }
                int count = dictionary.count;
                Entry[] entries = dictionary.entries;
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].hashCode >= 0)
                    {
                        array[index++] = entries[i].value;
                    }
                }
            }


            void ICollection<TValue>.Add(TValue item)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);
            }


            bool ICollection<TValue>.Remove(TValue item)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);
                return false;
            }


            void ICollection<TValue>.Clear()
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);
            }


            bool ICollection<TValue>.Contains(TValue item)
            {
                return dictionary.ContainsValue(item);
            }


            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            /// <summary>
            ///         Возвращает перечислитель, который осуществляет итерацию по коллекции.
            ///       </summary>
            /// <returns>
            ///         Объект <see cref="T:System.Collections.IEnumerator" />, который может использоваться для итерации элементов коллекции.
            ///       </returns>

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            /// <summary>
            ///         Копирует элементы коллекции <see cref="T:System.Collections.ICollection" /> в массив <see cref="T:System.Array" />, начиная с указанного индекса массива <see cref="T:System.Array" />.
            ///       </summary>
            /// <param name="array">
            ///           Одномерный массив <see cref="T:System.Array" />, в который копируются элементы из интерфейса <see cref="T:System.Collections.ICollection" />.
            ///            Массив <see cref="T:System.Array" /> должен иметь индексацию, начинающуюся с нуля.
            ///         </param>
            /// <param name="index">
            ///           Отсчитываемый от нуля индекс в массиве <paramref name="array" />, указывающий начало копирования.
            ///         </param>
            /// <exception cref="T:System.ArgumentNullException">
            ///             Свойство <paramref name="array" /> имеет значение <see langword="null" />.
            ///           </exception>
            /// <exception cref="T:System.ArgumentOutOfRangeException">
            ///             Значение параметра <paramref name="index" /> меньше нуля.
            ///           </exception>
            /// <exception cref="T:System.ArgumentException">
            ///             Массив <paramref name="array" /> является многомерным.
            ///
            ///             -или-
            ///
            ///             Массив <paramref name="array" /> не имеет индексации, начинающейся с нуля.
            ///
            ///             -или-
            ///
            ///             Количество элементов в исходной коллекции <see cref="T:System.Collections.ICollection" /> больше, чем свободное пространство от <paramref name="index" /> до конца массива назначения <paramref name="array" />.
            ///
            ///             -или-
            ///
            ///             Тип источника <see cref="T:System.Collections.ICollection" /> не может быть автоматически приведен к типу массива назначения <paramref name="array" />.
            ///           </exception>

            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                }
                if (array.Rank != 1)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
                }
                if (array.GetLowerBound(0) != 0)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
                }
                if (index < 0 || index > array.Length)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
                }
                if (array.Length - index < dictionary.Count)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
                }
                TValue[] array2 = array as TValue[];
                if (array2 != null)
                {
                    CopyTo(array2, index);
                }
                else
                {
                    object[] array3 = array as object[];
                    if (array3 == null)
                    {
                        ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
                    }
                    int count = dictionary.count;
                    Entry[] entries = dictionary.entries;
                    try
                    {
                        for (int i = 0; i < count; i++)
                        {
                            if (entries[i].hashCode >= 0)
                            {
                                array3[index++] = entries[i].value;
                            }
                        }
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
                    }
                }
            }
        }

        private int[] buckets;

        private Entry[] entries;

        private int count;

        private int version;

        private int freeList;

        private int freeCount;

        private IEqualityComparer<TKey> comparer;

        private KeyCollection keys;

        private ValueCollection values;

        private object _syncRoot;

        private const string VersionName = "Version";

        private const string HashSizeName = "HashSize";

        private const string KeyValuePairsName = "KeyValuePairs";

        private const string ComparerName = "Comparer";

        /// <summary>
        ///         Возвращает интерфейс <see cref="T:System.Collections.Generic.IEqualityComparer`1" />, используемый для установления равенства ключей словаря.
        ///       </summary>
        /// <returns>
        ///         Реализация универсального интерфейса <see cref="T:System.Collections.Generic.IEqualityComparer`1" />, используемая для установления равенства ключей текущего словаря <see cref="T:System.Collections.Generic.Dictionary`2" /> и для задания хэш-кодов ключей.
        ///       </returns>

        public IEqualityComparer<TKey> Comparer
        {

            get
            {
                return comparer;
            }
        }

        /// <summary>
        ///         Возвращает число пар "ключ-значение", содержащихся в словаре <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///       </summary>
        /// <returns>
        ///         Число пар "ключ-значение", содержащихся в словаре <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///       </returns>

        public int Count
        {

            get
            {
                return count - freeCount;
            }
        }

        /// <summary>
        ///         Возвращает коллекцию, содержащую ключи из словаря <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///       </summary>
        /// <returns>
        ///         Интерфейс <see cref="T:System.Collections.Generic.Dictionary`2.KeyCollection" />, содержащий ключи из <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///       </returns>

        public KeyCollection Keys
        {

            get
            {
                if (keys == null)
                {
                    keys = new KeyCollection(this);
                }
                return keys;
            }
        }


        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {

            get
            {
                if (keys == null)
                {
                    keys = new KeyCollection(this);
                }
                return keys;
            }
        }


        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {

            get
            {
                if (keys == null)
                {
                    keys = new KeyCollection(this);
                }
                return keys;
            }
        }

        /// <summary>
        ///         Возвращает коллекцию, содержащую значения из словаря <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///       </summary>
        /// <returns>
        ///         Коллекция <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" />, содержащая значения из словаря <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///       </returns>

        public ValueCollection Values
        {

            get
            {
                if (values == null)
                {
                    values = new ValueCollection(this);
                }
                return values;
            }
        }


        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {

            get
            {
                if (values == null)
                {
                    values = new ValueCollection(this);
                }
                return values;
            }
        }


        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {

            get
            {
                if (values == null)
                {
                    values = new ValueCollection(this);
                }
                return values;
            }
        }

        /// <summary>
        ///         Возвращает или задает значение, связанное с указанным ключом.
        ///       </summary>
        /// <param name="key">
        ///           Ключ, значение которого требуется получить или задать.
        ///         </param>
        /// <returns>
        ///         Значение, связанное с указанным ключом.
        ///          Если указанный ключ не найден, операция получения создает исключение <see cref="T:System.Collections.Generic.KeyNotFoundException" />, а операция задания значения создает новый элемент с указанным ключом.
        ///       </returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///             Свойство <paramref name="key" /> имеет значение <see langword="null" />.
        ///           </exception>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">
        ///             Свойство получено, а <paramref name="key" /> не существует в коллекции.
        ///           </exception>

        public TValue this[TKey key]
        {

            get
            {
                int num = FindEntry(key);
                if (num >= 0)
                {
                    return entries[num].value;
                }
                ThrowHelper.ThrowKeyNotFoundException();
                return default(TValue);
            }

            set
            {
                Insert(key, value, false);
            }
        }


        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {

            get
            {
                return false;
            }
        }

        /// <summary>
        ///         Возвращает значение, показывающее, является ли доступ к коллекции <see cref="T:System.Collections.ICollection" /> синхронизированным (потокобезопасным).
        ///       </summary>
        /// <returns>
        ///         <see langword="true" />, если доступ к классу <see cref="T:System.Collections.ICollection" /> является синхронизированным (потокобезопасным); в противном случае — <see langword="false" />.
        ///           В используемой по умолчанию реализации <see cref="T:System.Collections.Generic.Dictionary`2" /> это свойство всегда возвращает значение <see langword="false" />.
        ///       </returns>

        bool ICollection.IsSynchronized
        {

            get
            {
                return false;
            }
        }

        /// <summary>
        ///         Получает объект, с помощью которого можно синхронизировать доступ к коллекции <see cref="T:System.Collections.ICollection" />.
        ///       </summary>
        /// <returns>
        ///         Объект, который может использоваться для синхронизации доступа к <see cref="T:System.Collections.ICollection" />.
        ///       </returns>

        object ICollection.SyncRoot
        {

            get
            {
                if (_syncRoot == null)
                {
                    Interlocked.CompareExchange<object>(ref _syncRoot, new object(), (object)null);
                }
                return _syncRoot;
            }
        }

        /// <summary>
        ///         Получает значение, указывающее, имеет ли список <see cref="T:System.Collections.IDictionary" /> фиксированный размер.
        ///       </summary>
        /// <returns>
        ///         Значение <see langword="true" />, если словарь <see cref="T:System.Collections.IDictionary" /> имеет фиксированный размер; в противном случае — значение <see langword="false" />.
        ///           В используемой по умолчанию реализации <see cref="T:System.Collections.Generic.Dictionary`2" /> это свойство всегда возвращает значение <see langword="false" />.
        ///       </returns>

        bool IDictionary.IsFixedSize
        {

            get
            {
                return false;
            }
        }

        /// <summary>
        ///         Получает значение, указывающее, является ли объект <see cref="T:System.Collections.IDictionary" /> доступным только для чтения.
        ///       </summary>
        /// <returns>
        ///         Значение <see langword="true" />, если интерфейс <see cref="T:System.Collections.IDictionary" /> доступен только для чтения; в противном случае — значение <see langword="false" />.
        ///           В используемой по умолчанию реализации <see cref="T:System.Collections.Generic.Dictionary`2" /> это свойство всегда возвращает значение <see langword="false" />.
        ///       </returns>

        bool IDictionary.IsReadOnly
        {

            get
            {
                return false;
            }
        }

        /// <summary>
        ///         Возвращает интерфейс <see cref="T:System.Collections.ICollection" />, содержащий ключи <see cref="T:System.Collections.IDictionary" />.
        ///       </summary>
        /// <returns>
        ///         Интерфейс <see cref="T:System.Collections.ICollection" />, содержащий ключи из <see cref="T:System.Collections.IDictionary" />.
        ///       </returns>

        ICollection IDictionary.Keys
        {

            get
            {
                return Keys;
            }
        }

        /// <summary>
        ///         Возвращает интерфейс <see cref="T:System.Collections.ICollection" />, содержащий значения из <see cref="T:System.Collections.IDictionary" />.
        ///       </summary>
        /// <returns>
        ///         Коллекция <see cref="T:System.Collections.ICollection" />, содержащая значения из словаря <see cref="T:System.Collections.IDictionary" />.
        ///       </returns>

        ICollection IDictionary.Values
        {

            get
            {
                return Values;
            }
        }

        /// <summary>
        ///         Возвращает или задает значение с указанным ключом.
        ///       </summary>
        /// <param name="key">
        ///           Ключ значения, которое необходимо получить.
        ///         </param>
        /// <returns>
        ///         Значение, связанное с указанным ключом, или <see langword="null" />, если <paramref name="key" /> отсутствует в словаре или <paramref name="key" /> имеет тип, который не допускает присваивание типу ключа <paramref name="TKey" /> коллекции <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///       </returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///             Свойство <paramref name="key" /> имеет значение <see langword="null" />.
        ///           </exception>
        /// <exception cref="T:System.ArgumentException">
        ///             Назначается значение и <paramref name="key" /> относится к типу, который не допускает присваивание типу ключа <paramref name="TKey" /> из <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///
        ///             -или-
        ///
        ///             Назначается значение и <paramref name="value" /> имеет тип, который не может быть назначен типу значения <paramref name="TValue" /> из <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///           </exception>

        object IDictionary.this[object key]
        {

            get
            {
                if (IsCompatibleKey(key))
                {
                    int num = FindEntry((TKey)key);
                    if (num >= 0)
                    {
                        return entries[num].value;
                    }
                }
                return null;
            }

            set
            {
                if (key == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
                }
                ThrowHelper.IfNullAndNullsAreIllegalThenThrow<TValue>(value, ExceptionArgument.value);
                try
                {
                    TKey key2 = (TKey)key;
                    try
                    {
                        this[key2] = (TValue)value;
                    }
                    catch (InvalidCastException)
                    {
                        ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(TValue));
                    }
                }
                catch (InvalidCastException)
                {
                    ThrowHelper.ThrowWrongKeyTypeArgumentException(key, typeof(TKey));
                }
            }
        }

        /// <summary>
        ///         Инициализирует новый пустой экземпляр класса <see cref="T:System.Collections.Generic.Dictionary`2" />, имеющий начальную емкость по умолчанию и использующий функцию сравнения по умолчанию, проверяющую равенство для данного типа ключа.
        ///       </summary>

        public MultiValueDictionary()
            : this(0, (IEqualityComparer<TKey>)null)
        {
        }

        /// <summary>
        ///         Инициализирует новый пустой экземпляр класса <see cref="T:System.Collections.Generic.Dictionary`2" />, имеющий заданную начальную емкость и использующий функцию сравнения по умолчанию, проверяющую равенство для данного типа ключа.
        ///       </summary>
        /// <param name="capacity">
        ///           Начальное количество элементов, которое может содержать коллекция <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///         </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///             Значение параметра <paramref name="capacity" /> меньше 0.
        ///           </exception>

        public MultiValueDictionary(int capacity)
            : this(capacity, (IEqualityComparer<TKey>)null)
        {
        }

        /// <summary>
        ///         Инициализирует новый пустой экземпляр класса <see cref="T:System.Collections.Generic.Dictionary`2" /> начальной емкостью по умолчанию, использующий указанную функцию сравнения <see cref="T:System.Collections.Generic.IEqualityComparer`1" />.
        ///       </summary>
        /// <param name="comparer">
        ///           Реализация <see cref="T:System.Collections.Generic.IEqualityComparer`1" />, которую следует использовать при сравнении ключей, или <see langword="null" />, если для данного типа ключа должна использоваться реализация <see cref="T:System.Collections.Generic.EqualityComparer`1" /> по умолчанию.
        ///         </param>

        public MultiValueDictionary(IEqualityComparer<TKey> comparer)
            : this(0, comparer)
        {
        }

        /// <summary>
        ///         Инициализирует новый пустой экземпляр класса <see cref="T:System.Collections.Generic.Dictionary`2" /> заданной начальной емкостью, использующий указанную функцию сравнения <see cref="T:System.Collections.Generic.IEqualityComparer`1" />.
        ///       </summary>
        /// <param name="capacity">
        ///           Начальное количество элементов, которое может содержать коллекция <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///         </param>
        /// <param name="comparer">
        ///           Реализация <see cref="T:System.Collections.Generic.IEqualityComparer`1" />, которую следует использовать при сравнении ключей, или <see langword="null" />, если для данного типа ключа должна использоваться реализация <see cref="T:System.Collections.Generic.EqualityComparer`1" /> по умолчанию.
        ///         </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///             Значение параметра <paramref name="capacity" /> меньше 0.
        ///           </exception>

        public MultiValueDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            if (capacity < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity);
            }
            if (capacity > 0)
            {
                Initialize(capacity);
            }
            this.comparer = (comparer ?? EqualityComparer<TKey>.Default);
        }

        /// <summary>
        ///         Инициализирует новый экземпляр класса <see cref="T:System.Collections.Generic.Dictionary`2" />, который содержит элементы, скопированные из заданной коллекции <see cref="T:System.Collections.Generic.IDictionary`2" />, и использует функцию сравнения по умолчанию, проверяющую равенство для данного типа ключа.
        ///       </summary>
        /// <param name="dictionary">
        ///           Объект <see cref="T:System.Collections.Generic.IDictionary`2" />, элементы которого копируются в новый объект <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///         </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///             Свойство <paramref name="dictionary" /> имеет значение <see langword="null" />.
        ///           </exception>
        /// <exception cref="T:System.ArgumentException">
        ///             <paramref name="dictionary" /> содержит один или несколько ключей-дубликатов.
        ///           </exception>

        public MultiValueDictionary(IDictionary<TKey, TValue> dictionary)
            : this(dictionary, (IEqualityComparer<TKey>)null)
        {
        }

        /// <summary>
        ///         Инициализирует новый экземпляр класса <see cref="T:System.Collections.Generic.Dictionary`2" />, который содержит элементы, скопированные из заданной коллекции <see cref="T:System.Collections.Generic.IDictionary`2" />, и использует указанный интерфейс <see cref="T:System.Collections.Generic.IEqualityComparer`1" />.
        ///       </summary>
        /// <param name="dictionary">
        ///           Объект <see cref="T:System.Collections.Generic.IDictionary`2" />, элементы которого копируются в новый объект <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///         </param>
        /// <param name="comparer">
        ///           Реализация <see cref="T:System.Collections.Generic.IEqualityComparer`1" />, которую следует использовать при сравнении ключей, или <see langword="null" />, если для данного типа ключа должна использоваться реализация <see cref="T:System.Collections.Generic.EqualityComparer`1" /> по умолчанию.
        ///         </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///             Свойство <paramref name="dictionary" /> имеет значение <see langword="null" />.
        ///           </exception>
        /// <exception cref="T:System.ArgumentException">
        ///             <paramref name="dictionary" /> содержит один или несколько ключей-дубликатов.
        ///           </exception>

        public MultiValueDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
            : this((dictionary != null) ? dictionary.Count : 0, comparer)
        {
            if (dictionary == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);
            }
            foreach (KeyValuePair<TKey, TValue> item in dictionary)
            {
                Add(item.Key, item.Value);
            }
        }

        /// <summary>
        ///         Инициализирует новый экземпляр класса <see cref="T:System.Collections.Generic.Dictionary`2" /> с сериализованными данными.
        ///       </summary>
        /// <param name="info">
        ///           Объект <see cref="T:System.Runtime.Serialization.SerializationInfo" />, который содержит сведения, требуемые для сериализации коллекции <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///         </param>
        /// <param name="context">
        ///           Структура <see cref="T:System.Runtime.Serialization.StreamingContext" />, содержащая исходный объект и объект назначения для сериализованного потока, связанного с коллекцией <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///         </param>
        //protected Dictionary(SerializationInfo info, StreamingContext context)
        //{
        //	HashHelpers.SerializationInfoTable.Add(this, info);
        //}

        /// <summary>
        ///         Добавляет указанные ключ и значение в словарь.
        ///       </summary>
        /// <param name="key">
        ///           Ключ добавляемого элемента.
        ///         </param>
        /// <param name="value">
        ///           Добавляемое значение элемента.
        ///            Для ссылочных типов допускается значение <see langword="null" />.
        ///         </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///             Свойство <paramref name="key" /> имеет значение <see langword="null" />.
        ///           </exception>
        /// <exception cref="T:System.ArgumentException">
        ///             Элемент с таким ключом уже существует в <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///           </exception>

        public void Add(TKey key, TValue value)
        {
            Insert(key, value, true);
        }


        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
        {
            Add(keyValuePair.Key, keyValuePair.Value);
        }


        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
        {
            int num = FindEntry(keyValuePair.Key);
            if (num >= 0 && EqualityComparer<TValue>.Default.Equals(entries[num].value, keyValuePair.Value))
            {
                return true;
            }
            return false;
        }


        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            int num = FindEntry(keyValuePair.Key);
            if (num >= 0 && EqualityComparer<TValue>.Default.Equals(entries[num].value, keyValuePair.Value))
            {
                Remove(keyValuePair.Key);
                return true;
            }
            return false;
        }

        /// <summary>
        ///         Удаляет все ключи и значения из словаря <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///       </summary>

        public void Clear()
        {
            if (count > 0)
            {
                for (int i = 0; i < buckets.Length; i++)
                {
                    buckets[i] = -1;
                }
                Array.Clear(entries, 0, count);
                freeList = -1;
                count = 0;
                freeCount = 0;
                version++;
            }
        }

        /// <summary>
        ///         Определяет, содержится ли указанный ключ в словаре <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///       </summary>
        /// <param name="key">
        ///           Ключ, который требуется найти в <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///         </param>
        /// <returns>
        ///         <see langword="true" />, если <see cref="T:System.Collections.Generic.Dictionary`2" /> содержит элемент с указанным ключом, в противном случае — <see langword="false" />.
        ///       </returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///             Свойство <paramref name="key" /> имеет значение <see langword="null" />.
        ///           </exception>

        public bool ContainsKey(TKey key)
        {
            return FindEntry(key) >= 0;
        }

        /// <summary>
        ///         Определяет, содержит ли коллекция <see cref="T:System.Collections.Generic.Dictionary`2" /> указанное значение.
        ///       </summary>
        /// <param name="value">
        ///           Значение, которое требуется найти в словаре <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///            Для ссылочных типов допускается значение <see langword="null" />.
        ///         </param>
        /// <returns>
        ///         Значение <see langword="true" />, если <see cref="T:System.Collections.Generic.Dictionary`2" /> содержит элемент с указанным значением; в противном случае — значение <see langword="false" />.
        ///       </returns>

        public bool ContainsValue(TValue value)
        {
            if (value == null)
            {
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].hashCode >= 0 && entries[i].value == null)
                    {
                        return true;
                    }
                }
            }
            else
            {
                EqualityComparer<TValue> @default = EqualityComparer<TValue>.Default;
                for (int j = 0; j < count; j++)
                {
                    if (entries[j].hashCode >= 0 && @default.Equals(entries[j].value, value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            if (array == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            }
            if (index < 0 || index > array.Length)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (array.Length - index < Count)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
            }
            int num = count;
            Entry[] array2 = entries;
            for (int i = 0; i < num; i++)
            {
                if (array2[i].hashCode >= 0)
                {
                    array[index++] = new KeyValuePair<TKey, TValue>(array2[i].key, array2[i].value);
                }
            }
        }

        /// <summary>
        ///         Возвращает перечислитель, осуществляющий перебор элементов списка <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///       </summary>
        /// <returns>
        ///         Структура <see cref="T:System.Collections.Generic.Dictionary`2.Enumerator" /> для <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///       </returns>

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this, 2);
        }


        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return new Enumerator(this, 2);
        }

        /// <summary>
        ///         Реализует интерфейс <see cref="T:System.Runtime.Serialization.ISerializable" /> и возвращает данные, необходимые для сериализации экземпляра <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///       </summary>
        /// <param name="info">
        ///           Объект <see cref="T:System.Runtime.Serialization.SerializationInfo" />, который содержит сведения, требуемые для сериализации экземпляра <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///         </param>
        /// <param name="context">
        ///           Структура <see cref="T:System.Runtime.Serialization.StreamingContext" />, содержащая исходный и конечный объекты сериализованного потока, связанного с экземпляром <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///         </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///             Свойство <paramref name="info" /> имеет значение <see langword="null" />.
        ///           </exception>
        [SecurityCritical]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.info);
            }
            info.AddValue("Version", version);
            info.AddValue("Comparer", HashHelpers.GetEqualityComparerForSerialization(comparer), typeof(IEqualityComparer<TKey>));
            info.AddValue("HashSize", (buckets != null) ? buckets.Length : 0);
            if (buckets != null)
            {
                KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[Count];
                CopyTo(array, 0);
                info.AddValue("KeyValuePairs", array, typeof(KeyValuePair<TKey, TValue>[]));
            }
        }

        private int FindEntry(TKey key)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            }
            if (buckets != null)
            {
                int num = comparer.GetHashCode(key) & 0x7FFFFFFF;
                for (int num2 = buckets[num % buckets.Length]; num2 >= 0; num2 = entries[num2].next)
                {
                    if (entries[num2].hashCode == num && comparer.Equals(entries[num2].key, key))
                    {
                        return num2;
                    }
                }
            }
            return -1;
        }

        private void Initialize(int capacity)
        {
            int prime = HashHelpers.GetPrime(capacity);
            buckets = new int[prime];
            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i] = -1;
            }
            entries = new Entry[prime];
            freeList = -1;
        }

        private void Insert(TKey key, TValue value, bool add)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            }
            if (buckets == null)
            {
                Initialize(0);
            }
            int num = comparer.GetHashCode(key) & 0x7FFFFFFF;
            int num2 = num % buckets.Length;
            int num3 = 0;
            for (int num4 = buckets[num2]; num4 >= 0; num4 = entries[num4].next)
            {
                //if (entries[num4].hashCode == num && comparer.Equals(entries[num4].key, key))
                //{
                //	if (add)
                //	{
                //		ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_AddingDuplicate);
                //	}
                //	entries[num4].value = value;
                //	version++;
                //	return;
                //}
                num3++;
            }
            int num5;
            if (freeCount > 0)
            {
                num5 = freeList;
                freeList = entries[num5].next;
                freeCount--;
            }
            else
            {
                if (count == entries.Length)
                {
                    Resize();
                    num2 = num % buckets.Length;
                }
                num5 = count;
                count++;
            }
            entries[num5].hashCode = num;
            entries[num5].next = buckets[num2];
            entries[num5].key = key;
            entries[num5].value = value;
            buckets[num2] = num5;
            version++;
            if (num3 > 100 && HashHelpers.IsWellKnownEqualityComparer(comparer))
            {
                comparer = (IEqualityComparer<TKey>)HashHelpers.GetRandomizedEqualityComparer(comparer);
                Resize(entries.Length, true);
            }
        }

        /// <summary>
        ///         Реализует интерфейс <see cref="T:System.Runtime.Serialization.ISerializable" /> и вызывает событие десериализации при завершении десериализации.
        ///       </summary>
        /// <param name="sender">
        ///           Источник события десериализации.
        ///         </param>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        ///             <see cref="T:System.Runtime.Serialization.SerializationInfo" /> Объект, связанный с текущим <see cref="T:System.Collections.Generic.Dictionary`2" /> экземпляр является недопустимым.
        ///           </exception>
        public virtual void OnDeserialization(object sender)
        {
            HashHelpers.SerializationInfoTable.TryGetValue(this, out SerializationInfo value);
            if (value != null)
            {
                int @int = value.GetInt32("Version");
                int int2 = value.GetInt32("HashSize");
                comparer = (IEqualityComparer<TKey>)value.GetValue("Comparer", typeof(IEqualityComparer<TKey>));
                if (int2 != 0)
                {
                    buckets = new int[int2];
                    for (int i = 0; i < buckets.Length; i++)
                    {
                        buckets[i] = -1;
                    }
                    entries = new Entry[int2];
                    freeList = -1;
                    KeyValuePair<TKey, TValue>[] array = (KeyValuePair<TKey, TValue>[])value.GetValue("KeyValuePairs", typeof(KeyValuePair<TKey, TValue>[]));
                    if (array == null)
                    {
                        ThrowHelper.ThrowSerializationException(ExceptionResource.Serialization_MissingKeys);
                    }
                    for (int j = 0; j < array.Length; j++)
                    {
                        if (array[j].Key == null)
                        {
                            ThrowHelper.ThrowSerializationException(ExceptionResource.Serialization_NullKey);
                        }
                        Insert(array[j].Key, array[j].Value, true);
                    }
                }
                else
                {
                    buckets = null;
                }
                version = @int;
                HashHelpers.SerializationInfoTable.Remove(this);
            }
        }

        private void Resize()
        {
            Resize(HashHelpers.ExpandPrime(count), false);
        }

        private void Resize(int newSize, bool forceNewHashCodes)
        {
            int[] array = new int[newSize];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = -1;
            }
            Entry[] array2 = new Entry[newSize];
            Array.Copy(entries, 0, array2, 0, count);
            if (forceNewHashCodes)
            {
                for (int j = 0; j < count; j++)
                {
                    if (array2[j].hashCode != -1)
                    {
                        array2[j].hashCode = (comparer.GetHashCode(array2[j].key) & 0x7FFFFFFF);
                    }
                }
            }
            for (int k = 0; k < count; k++)
            {
                if (array2[k].hashCode >= 0)
                {
                    int num = array2[k].hashCode % newSize;
                    array2[k].next = array[num];
                    array[num] = k;
                }
            }
            buckets = array;
            entries = array2;
        }

        /// <summary>
        ///         Удаляет значение с указанным ключом из <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///       </summary>
        /// <param name="key">
        ///           Ключ элемента, который требуется удалить.
        ///         </param>
        /// <returns>
        ///         Значение <see langword="true" />, если элемент был найден и удален; в противном случае — значение <see langword="false" />.
        ///           Этот метод возвращает значение <see langword="false" />, если ключ <paramref name="key" /> не удалось найти в <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///       </returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///             Свойство <paramref name="key" /> имеет значение <see langword="null" />.
        ///           </exception>

        public bool Remove(TKey key)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            }
            if (buckets != null)
            {
                int num = comparer.GetHashCode(key) & 0x7FFFFFFF;
                int num2 = num % buckets.Length;
                int num3 = -1;
                for (int num4 = buckets[num2]; num4 >= 0; num4 = entries[num4].next)
                {
                    if (entries[num4].hashCode == num && comparer.Equals(entries[num4].key, key))
                    {
                        if (num3 < 0)
                        {
                            buckets[num2] = entries[num4].next;
                        }
                        else
                        {
                            entries[num3].next = entries[num4].next;
                        }
                        entries[num4].hashCode = -1;
                        entries[num4].next = freeList;
                        entries[num4].key = default(TKey);
                        entries[num4].value = default(TValue);
                        freeList = num4;
                        freeCount++;
                        version++;
                        return true;
                    }
                    num3 = num4;
                }
            }
            return false;
        }

        /// <summary>
        ///         Возвращает значение, связанное с заданным ключом.
        ///       </summary>
        /// <param name="key">
        ///           Ключ значения, которое необходимо получить.
        ///         </param>
        /// <param name="value">
        ///           Этот метод возвращает значение, связанное с указанным ключом, если он найден; в противном случае — значение по умолчанию для типа параметра <paramref name="value" />.
        ///            Этот параметр передается неинициализированным.
        ///         </param>
        /// <returns>
        ///         <see langword="true" />, если <see cref="T:System.Collections.Generic.Dictionary`2" /> содержит элемент с указанным ключом, в противном случае — <see langword="false" />.
        ///       </returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///             Свойство <paramref name="key" /> имеет значение <see langword="null" />.
        ///           </exception>

        public bool TryGetValue(TKey key, out TValue value)
        {
            int num = FindEntry(key);
            if (num >= 0)
            {
                value = entries[num].value;
                return true;
            }
            value = default(TValue);
            return false;
        }

        internal TValue GetValueOrDefault(TKey key)
        {
            int num = FindEntry(key);
            if (num >= 0)
            {
                return entries[num].value;
            }
            return default(TValue);
        }


        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            CopyTo(array, index);
        }

        /// <summary>
        ///         Копирует элементы коллекции <see cref="T:System.Collections.Generic.ICollection`1" /> в массив, начиная с указанного индекса массива.
        ///       </summary>
        /// <param name="array">
        ///           Одномерный массив, в который копируются элементы из коллекции <see cref="T:System.Collections.Generic.ICollection`1" />.
        ///            Индекс в массиве должен начинаться с нуля.
        ///         </param>
        /// <param name="index">
        ///           Отсчитываемый от нуля индекс в массиве <paramref name="array" />, указывающий начало копирования.
        ///         </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///             Свойство <paramref name="array" /> имеет значение <see langword="null" />.
        ///           </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///             Значение параметра <paramref name="index" /> меньше 0.
        ///           </exception>
        /// <exception cref="T:System.ArgumentException">
        ///             Массив <paramref name="array" /> является многомерным.
        ///
        ///             -или-
        ///
        ///             Массив <paramref name="array" /> не имеет индексации, начинающейся с нуля.
        ///
        ///             -или-
        ///
        ///             Количество элементов в исходной коллекции <see cref="T:System.Collections.Generic.ICollection`1" /> больше, чем свободное пространство от <paramref name="index" /> до конца массива назначения <paramref name="array" />.
        ///
        ///             -или-
        ///
        ///             Тип источника <see cref="T:System.Collections.Generic.ICollection`1" /> не может быть автоматически приведен к типу массива назначения <paramref name="array" />.
        ///           </exception>

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            }
            if (array.Rank != 1)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
            }
            if (array.GetLowerBound(0) != 0)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
            }
            if (index < 0 || index > array.Length)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (array.Length - index < Count)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
            }
            KeyValuePair<TKey, TValue>[] array2 = array as KeyValuePair<TKey, TValue>[];
            if (array2 != null)
            {
                CopyTo(array2, index);
            }
            else if (array is DictionaryEntry[])
            {
                DictionaryEntry[] array3 = array as DictionaryEntry[];
                Entry[] array4 = entries;
                for (int i = 0; i < count; i++)
                {
                    if (array4[i].hashCode >= 0)
                    {
                        array3[index++] = new DictionaryEntry(array4[i].key, array4[i].value);
                    }
                }
            }
            else
            {
                object[] array5 = array as object[];
                if (array5 == null)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
                }
                try
                {
                    int num2 = count;
                    Entry[] array6 = entries;
                    for (int j = 0; j < num2; j++)
                    {
                        if (array6[j].hashCode >= 0)
                        {
                            array5[index++] = new KeyValuePair<TKey, TValue>(array6[j].key, array6[j].value);
                        }
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
                }
            }
        }

        /// <summary>
        ///         Возвращает перечислитель, выполняющий перебор элементов в коллекции.
        ///       </summary>
        /// <returns>
        ///         Объект <see cref="T:System.Collections.IEnumerator" />, который может использоваться для итерации элементов коллекции.
        ///       </returns>

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this, 2);
        }

        private static bool IsCompatibleKey(object key)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            }
            return key is TKey;
        }

        /// <summary>
        ///         Добавляет указанные ключ и значение в словарь.
        ///       </summary>
        /// <param name="key">
        ///           Объект, который используется в качестве ключа.
        ///         </param>
        /// <param name="value">
        ///           Объект, который используется в качестве значения.
        ///         </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///             Свойство <paramref name="key" /> имеет значение <see langword="null" />.
        ///           </exception>
        /// <exception cref="T:System.ArgumentException">
        ///             <paramref name="key" /> имеет тип, который не допускает присваивание типу ключа <paramref name="TKey" /> из <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///
        ///             -или-
        ///
        ///             <paramref name="value" /> имеет тип, который не может быть назначен <paramref name="TValue" />, тип значения в <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///
        ///             -или-
        ///
        ///             Значение с тем же ключом уже существует в <see cref="T:System.Collections.Generic.Dictionary`2" />.
        ///           </exception>

        void IDictionary.Add(object key, object value)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            }
            ThrowHelper.IfNullAndNullsAreIllegalThenThrow<TValue>(value, ExceptionArgument.value);
            try
            {
                TKey key2 = (TKey)key;
                try
                {
                    Add(key2, (TValue)value);
                }
                catch (InvalidCastException)
                {
                    ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(TValue));
                }
            }
            catch (InvalidCastException)
            {
                ThrowHelper.ThrowWrongKeyTypeArgumentException(key, typeof(TKey));
            }
        }

        /// <summary>
        ///         Определяет, содержится ли элемент с указанным ключом в <see cref="T:System.Collections.IDictionary" />.
        ///       </summary>
        /// <param name="key">
        ///           Ключ, который требуется найти в <see cref="T:System.Collections.IDictionary" />.
        ///         </param>
        /// <returns>
        ///         <see langword="true" />, если <see cref="T:System.Collections.IDictionary" /> содержит элемент с указанным ключом, в противном случае — <see langword="false" />.
        ///       </returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///             Свойство <paramref name="key" /> имеет значение <see langword="null" />.
        ///           </exception>

        bool IDictionary.Contains(object key)
        {
            if (IsCompatibleKey(key))
            {
                return ContainsKey((TKey)key);
            }
            return false;
        }

        /// <summary>
        ///         Возвращает перечислитель <see cref="T:System.Collections.IDictionaryEnumerator" /> для словаря <see cref="T:System.Collections.IDictionary" />.
        ///       </summary>
        /// <returns>
        ///         Интерфейс <see cref="T:System.Collections.IDictionaryEnumerator" /> для <see cref="T:System.Collections.IDictionary" />.
        ///       </returns>

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new Enumerator(this, 1);
        }

        /// <summary>
        ///         Удаляет элемент с указанным ключом из <see cref="T:System.Collections.IDictionary" />.
        ///       </summary>
        /// <param name="key">
        ///           Ключ элемента, который требуется удалить.
        ///         </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///             Свойство <paramref name="key" /> имеет значение <see langword="null" />.
        ///           </exception>

        void IDictionary.Remove(object key)
        {
            if (IsCompatibleKey(key))
            {
                Remove((TKey)key);
            }
        }
    }
}