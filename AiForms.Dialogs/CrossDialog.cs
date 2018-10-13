using System;
using AiForms.Dialogs;
using AiForms.Dialogs.Abstractions;

namespace AiForms.Dialogs
{
    public class Dialog
    {
        static Lazy<IDialog> Implementation = new Lazy<IDialog>(() => CreateDialog(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

        public static IDialog Instance
        {
            get
            {
                var ret = Implementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }
                return ret;
            }
        }

        static IDialog CreateDialog()
        {
#if NETSTANDARD2_0
            return null;
#else
            return new DialogImplementation();
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
        }
    }
}
