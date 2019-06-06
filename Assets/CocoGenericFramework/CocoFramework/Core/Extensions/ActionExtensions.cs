using System;

namespace TabTale
{
    public static class ActionExtensions
    {
        /// <summary>
        /// Safe Execution of the Action
        /// </summary>
        public static void SafeFire(this Action _this)
        {
            if (_this != null)
                _this();
        }

        /// <summary>
        /// Safe Execution of the Action
        /// </summary>
        public static void SafeFire<T>(this Action<T> _this, T genParam)
        {
            if (_this != null)
                _this(genParam);
        }

        /// <summary>
        /// Safe Execution of the Action
        /// </summary>
        public static void SafeFire<T, TK>(this Action<T, TK> _this, T genParam1, TK genParam2)
        {
            if (_this != null)
                _this(genParam1,genParam2);
        }

        /// <summary>
        /// Safe Execution of the Action
        /// If you use this one - you may want to re-think your occupation as a programmer
        /// </summary>
        public static void SafeFire<T1, T2, T3>(this Action<T1, T2, T3> _this, T1 genParam1, T2 genParam2, T3 genParam3)
        {
            if (_this != null)
                _this(genParam1, genParam2, genParam3);
        } 

    }
}
