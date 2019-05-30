namespace GameFramework.Utility.Singleton
{
    public interface IHandleMessage
    {
        void HandleMessage(string msg, params object[] objs);
        object HandleMessageRetValue(string msg, params object[]objs);
    }
}