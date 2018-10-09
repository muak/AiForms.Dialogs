using System;
using AiForms.Extras.Abstractions;

namespace AiForms.Extras
{
    public class Toast
    {
        static Lazy<IToast> Implementation = new Lazy<IToast>(() => CreateToast(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

        public static IToast Instance
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

        static IToast CreateToast()
        {
#if NETSTANDARD2_0
            return null;
#else
            return new ToastImplementation();
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
        }
    }
}
