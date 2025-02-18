namespace Services.Updater
{
    public interface ILateUpdatable : IBaseUpdatable
    {
        void LateUpdate();
    }
}