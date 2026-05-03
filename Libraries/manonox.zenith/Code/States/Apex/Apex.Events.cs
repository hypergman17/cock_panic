namespace Zenith.States;

public partial class Apex {
    public interface IEvents : ISceneEvent<IEvents> {
        void OnJump() { }
    }

    public void Post(Action<IEvents> action) => IEvents.PostToGameObject(GameObject, action);
}