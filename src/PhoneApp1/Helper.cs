using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Facebook
{
    public static class UIThread
    {
        private static readonly Dispatcher Dispatcher;

        static UIThread()
        {
            // Store a reference to the current Dispatcher once per application
            Dispatcher = Deployment.Current.Dispatcher;
        }

        /// <summary>
        ///   Invokes the given action on the UI thread - if the current thread is the UI thread this will just invoke the action directly on
        ///   the current thread so it can be safely called without the calling method being aware of which thread it is on.
        /// </summary>
        public static void Invoke(Action action)
        {
            if (Dispatcher.CheckAccess())
                action.Invoke();
            else
                Dispatcher.BeginInvoke(action);
        }
    }

    public class SerializeHelper
    {
        public static void SaveSetting<T>(string fileName, T dataToSave)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    using (var stream = store.CreateFile(fileName))
                    {
                        var serializer = new DataContractSerializer(typeof(T));
                        serializer.WriteObject(stream, dataToSave);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return;
                }
            }
        }

        public static T LoadSetting<T>(string fileName)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!store.FileExists(fileName))
                    return default(T);
                try
                {
                    using (var stream = store.OpenFile(fileName, FileMode.Open, FileAccess.Read))
                    {
                        var serializer = new DataContractSerializer(typeof(T));
                        return (T)serializer.ReadObject(stream);
                    }
                }
                catch (Exception e)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(
                        () => MessageBox.Show(e.Message));
                    return default(T);
                }
            }
        }
    }
}
