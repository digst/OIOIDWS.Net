namespace Digst.OioIdws.Wsp.Wsdl.Bindings
{
    using System;
    using System.Collections;

    public sealed class BindingCollection<TValue, TParent> : CollectionBase
        where TValue : BindingNameItem<TParent>
    {
        #region CollectionBase

        Hashtable table;
        object parent;

        public BindingCollection(object parent) : base()
        {
            this.parent = parent;
        }

        private IDictionary Table
        {
            get { if (table == null) table = new Hashtable(); return table; }
        }

        protected override void OnInsertComplete(int index, object value)
        {
            AddValue(value);
        }

        protected override void OnRemove(int index, object value)
        {
            RemoveValue(value);
        }

        protected override void OnClear()
        {
            for (int i = 0; i < List.Count; i++)
            {
                RemoveValue(List[i]);
            }
        }

        protected override void OnSet(int index, object oldValue, object newValue)
        {
            RemoveValue(oldValue);
            AddValue(newValue);
        }

        void AddValue(object value)
        {
            string key = GetKey(value);
            if (key != null)
            {
                try
                {
                    Table.Add(key, value);
                }
                catch (Exception e)
                {
                    if (e is System.Threading.ThreadAbortException || e is StackOverflowException || e is OutOfMemoryException)
                    {
                        throw;
                    }
                    if (Table[key] != null)
                    {
                        throw new ArgumentException(GetDuplicateMessage(value.GetType(), key), e.InnerException);
                    }
                    else
                    {
                        throw e;
                    }
                }
            }
            SetParent(value, parent);
        }

        void RemoveValue(object value)
        {
            string key = GetKey(value);
            if (key != null) Table.Remove(key);
            SetParent(value, null);
        }

        static string GetDuplicateMessage(Type type, string elemName)
        {
            var message = String.Empty;

            if (type == typeof(System.Web.Services.Description.ServiceDescriptionFormatExtension))
                message = "Duplicate FormatExtension: " + elemName;
            else if (type == typeof(System.Web.Services.Description.OperationMessage))
                message = "Duplicate OperationMessage: " + elemName;
            else if (type == typeof(System.Web.Services.Description.Import))
                message = "Duplicate Import: " + elemName;
            else if (type == typeof(System.Web.Services.Description.Message))
                message = "Duplicate Message: " + elemName;
            else if (type == typeof(System.Web.Services.Description.Port))
                message = "Duplicate Port: " + elemName;
            else if (type == typeof(System.Web.Services.Description.PortType))
                message = "Duplicate PortType: " + elemName;
            else if (type == typeof(System.Web.Services.Description.Binding))
                message = "Duplicate Binding: " + elemName;
            else if (type == typeof(System.Web.Services.Description.Service))
                message = "Duplicate Service: " + elemName;
            else if (type == typeof(System.Web.Services.Description.MessagePart))
                message = "Duplicate MessagePart: " + elemName;
            else if (type == typeof(System.Web.Services.Description.OperationBinding))
                message = "Duplicate OperationBinding: " + elemName;
            else if (type == typeof(System.Web.Services.Description.FaultBinding))
                message = "Duplicate FaultBinding: " + elemName;
            else if (type == typeof(System.Web.Services.Description.Operation))
                message = "Duplicate Operation: " + elemName;
            else if (type == typeof(System.Web.Services.Description.OperationFault))
                message = "Duplicate OperationFault: " + elemName;
            else
                message = "Duplicate UnknownElement, type: " + elemName;

            return message;
        }

        #endregion

        #region ServiceDescriptionBaseCollection

        public TValue this[int index]
        {
            get { return (TValue)List[index]; }
            set { List[index] = value; }
        }

        public int Add(TValue value)
        {
            return List.Add(value);
        }

        public void Insert(int index, TValue value)
        {
            List.Insert(index, value);
        }

        public int IndexOf(TValue value)
        {
            return List.IndexOf(value);
        }

        public bool Contains(TValue value)
        {
            return List.Contains(value);
        }

        public void Remove(TValue value)
        {
            List.Remove(value);
        }

        public void CopyTo(TValue[] array, int index)
        {
            List.CopyTo(array, index);
        }

        public TValue this[string name]
        {
            get { return (TValue)Table[name]; }
        }

        private string GetKey(object value)
        {
            return ((TValue)value).Name;
        }

        private void SetParent(object value, object parent)
        {
            ((TValue)value).SetParent(
                (TParent)parent
            );
        }

        #endregion
    }
}