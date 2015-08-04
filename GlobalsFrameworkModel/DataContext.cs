using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GlobalsFramework.Actions;
using GlobalsFramework.Linq;
using GlobalsFramework.Utils.ConnectionManagement;
using GlobalsFramework.Validation;
using InterSystems.Globals;

namespace GlobalsFramework
{
    public class DataContext : IDisposable
    {
        private readonly Connection _connection;
        private readonly ConcurrentQueue<IEntityAction> _actionsQueue;
        private readonly ConcurrentBag<NodeReference> _createdReferences; 

        protected DataContext()
        {
            InitializeDbSetMembers();
            _connection = ConnectionManager.GetOpenedConnectionSyncronously();
            _actionsQueue = new ConcurrentQueue<IEntityAction>();
            _createdReferences = new ConcurrentBag<NodeReference>();
        }

        protected DataContext(string namespc, string user, string password)
        {
            InitializeDbSetMembers();
            _connection = ConnectionManager.GetOpenedConnectionSyncronously(namespc, user, password);
            _actionsQueue = new ConcurrentQueue<IEntityAction>();
            _createdReferences = new ConcurrentBag<NodeReference>();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Close()
        {
            Dispose();
        }

        public void SubmitChanges()
        {
            while (_actionsQueue.Count > 0)
            {
                IEntityAction action;
                _actionsQueue.TryDequeue(out action);
                action.Execute();
            }
        }

        internal void RegisterAction(IEntityAction action)
        {
            _actionsQueue.Enqueue(action);
        }

        internal Connection GetConnection()
        {
            return _connection;
        }

        internal NodeReference CreateNodeReference(string name)
        {
            var result = _connection.CreateNodeReference(name);
            RegisterOpenedReference(result);
            return result;
        }

        internal NodeReference CreateNodeReference()
        {
            var result = _connection.CreateNodeReference();
            RegisterOpenedReference(result);
            return result;
        }

        internal NodeReference CopyReference(NodeReference reference)
        {
            var result = CreateNodeReference();
            result.SetName(reference.GetName());

            var subscriptCount = reference.GetSubscriptCount();

            if (subscriptCount <= 0)
                return result;

            for (var position = 1; position <= subscriptCount; position++)
            {
                var subscript = reference.GetObjectSubscript(position);

                if (subscript is int)
                    result.AppendSubscript((int)subscript);
                else if (subscript is long)
                    result.AppendSubscript((long)subscript);
                else if (subscript is double)
                    result.AppendSubscript((double)subscript);
                else
                    result.AppendSubscript((string)subscript);
            }

            return result;
        }

        internal List<NodeReference> CopyReferences(IEnumerable<NodeReference> references)
        {
            return references.Select(CopyReference).ToList();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connection.Close();
                CloseReferences();
            }
        }

        private void InitializeDbSetMembers()
        {
            const BindingFlags constructorBindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            const BindingFlags memberBindingFlags = BindingFlags.Public | BindingFlags.Instance;

            var contextType = GetType();

            var dbSetFields = contextType
                .GetFields(memberBindingFlags)
                .Where(
                    f => f.FieldType.IsGenericType &&
                         f.FieldType
                             .GetInterfaces()
                             .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (IDbSet<>))
                );


            foreach (var dbSetField in dbSetFields)
            {
                var instanceType = dbSetField.FieldType;
                EntityValidator.ValidateDefinitionAndThrow(instanceType.GetGenericArguments().First());
                var instance = Activator.CreateInstance(instanceType, constructorBindingFlags, null, new object[] {this}, null);

                dbSetField.SetValue(this, instance);
            }

            var dbSetProperties = contextType
                .GetProperties(memberBindingFlags)
                .Where(
                    f => f.PropertyType.IsGenericType &&
                         f.PropertyType
                             .GetInterfaces()
                             .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (IDbSet<>))
                );

            foreach (var dbSetProperty in dbSetProperties)
            {
                var instanceType = dbSetProperty.PropertyType;
                EntityValidator.ValidateDefinitionAndThrow(instanceType.GetGenericArguments().First());
                var instance = Activator.CreateInstance(instanceType, constructorBindingFlags, null, new object[] { this }, null);

                dbSetProperty.SetValue(this, instance, null);
            }
        }

        private void RegisterOpenedReference(NodeReference reference)
        {
            _createdReferences.Add(reference);
        }

        private void CloseReferences()
        {
            foreach (var createdReference in _createdReferences)
            {
                createdReference.Close();
            }
        }
    }
}
