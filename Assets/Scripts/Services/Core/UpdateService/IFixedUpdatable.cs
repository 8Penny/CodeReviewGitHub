namespace Services.Updater
{
    public interface IFixedUpdatable : IBaseUpdatable
    {
        void FixedUpdate(float deltaTime);
    }
}