using System;

namespace Assman
{
    public enum ResourceModeCondition
    {
        Never,
        DebugOnly,
        ReleaseOnly,
        Always
    }

    public static class ResourceModeConditionExtensions
    {
        public static bool IsTrue(this ResourceModeCondition resourceModeCondition, ResourceMode resourceMode)
        {
            switch (resourceModeCondition)
            {
                case ResourceModeCondition.Never:
                    return false;
                case ResourceModeCondition.DebugOnly:
                    return resourceMode == ResourceMode.Debug;
                case ResourceModeCondition.ReleaseOnly:
                    return resourceMode == ResourceMode.Release;
                case ResourceModeCondition.Always:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException("resourceModeCondition");
            }
        }

        public static bool IsFalse(this ResourceModeCondition resourceModeCondition, ResourceMode resourceMode)
        {
            return !resourceModeCondition.IsTrue(resourceMode);
        }
    }
}