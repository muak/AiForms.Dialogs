using System;
namespace AiForms.Extras.Abstractions
{
    public static class Configurations
    {
        static LoadingConfig _LoadingConfig;
        static DialogConfig _DialogConfig;

        public static LoadingConfig LoadingConfig
        {
            get { return _LoadingConfig = _LoadingConfig ?? new LoadingConfig(); }
            set { _LoadingConfig = value; }
        }

        public static DialogConfig DialogConfig
        {
            get { return _DialogConfig = _DialogConfig ?? new DialogConfig(); }
            set { _DialogConfig = value; }
        }
    }
}
