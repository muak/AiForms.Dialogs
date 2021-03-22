using System;
using AiForms.Dialogs;
using AiForms.Dialogs.Abstractions;

namespace AiForms.Dialogs
{
    public class Loading
    {
        static Lazy<ILoading> Implementation = new Lazy<ILoading>(() => CreateLoading(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

        public static ILoading Instance
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

        static ILoading CreateLoading()
        {
#if XAMARIN_IOS
            return new LoadingImplementation();
#elif MONOANDROID
            return new LoadingImplementation();
#else
            return null;
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
        }
    }
}
