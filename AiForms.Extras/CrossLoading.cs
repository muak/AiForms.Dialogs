using System;
using AiForms.Extras.Abstractions;

namespace AiForms.Extras
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
#if NETSTANDARD2_0
        return null;
#else
            return new LoadingImplementation();
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
        }
    }
}
