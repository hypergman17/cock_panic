// namespace Zenith;

// public partial class Controller : Controller.IPreUpdateSubscriber, Controller.IPostUpdateSubscriber, Controller.IPreFixedUpdateSubscriber, Controller.IPostFixedUpdateSubscriber {
//     public class System : GameObjectSystem<System> {
//         public System(Scene scene) : base(scene) {
//             Listen(Stage.StartUpdate, -1, PreUpdate, "PreUpdate");
//             Listen(Stage.FinishUpdate, 1, PostUpdate, "PostUpdate");
//             Listen(Stage.StartUpdate, -1, PreFixedUpdate, "PreFixedUpdate");
//             Listen(Stage.FinishUpdate, 1, PostFixedUpdate, "PostFixedUpdate");
//         }

//         public void PreUpdate() {
//             Scene.GetAllComponents<IPreUpdateSubscriber>().ForEach(x => x.OnPreUpdate());
//         }

//         public void PostUpdate() {
//             Scene.GetAllComponents<IPostUpdateSubscriber>().ForEach(x => x.OnPostUpdate());
//         }

//         public void PreFixedUpdate() {
//             Scene.GetAllComponents<IPreFixedUpdateSubscriber>().ForEach(x => x.OnPreFixedUpdate());
//         }

//         public void PostFixedUpdate() {
//             Scene.GetAllComponents<IPostFixedUpdateSubscriber>().ForEach(x => x.OnPostFixedUpdate());
//         }
//     }

//     protected interface IPreUpdateSubscriber {
//         virtual void OnPreUpdate() { }
//     }

//     protected interface IPostUpdateSubscriber {
//         virtual void OnPostUpdate() { }
//     }

//     protected interface IPreFixedUpdateSubscriber {
//         virtual void OnPreFixedUpdate() { }
//     }
//     protected interface IPostFixedUpdateSubscriber {
//         virtual void OnPostFixedUpdate() { }
//     }
// }
