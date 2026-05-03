namespace Zenith.Utility;


[AttributeUsage(AttributeTargets.Property)]
[CodeGenerator(CodeGeneratorFlags.WrapPropertySet, nameof(OnlySystem.OnChangePropertySet), 10)]
public class FixedUpdateOnlyAttribute : Attribute {
    public FixedUpdateOnlyAttribute() { }
}

public class FixedUpdateOnlyException(PropertyDescription propertyDescription) : Exception(
    $"Attempt to set [FixedUpdateOnly] property `({propertyDescription.Name})` not at the FixedUpdate stage!"
) { }


[AttributeUsage(AttributeTargets.Property)]
[CodeGenerator(CodeGeneratorFlags.WrapPropertySet, nameof(OnlySystem.OnChangePropertySet), 10)]
public class UpdateOnlyAttribute : Attribute {
    public UpdateOnlyAttribute() { }
}

public class UpdateOnlyException(PropertyDescription propertyDescription) : Exception(
    $"Attempt to set [UpdateOnly] property `({propertyDescription.Name})` not at the Update stage!"
) { }

public static class OnlySystem {
    public static void OnChangePropertySet<T>(in WrappedPropertySet<T> p) {
        if (p.Object is not Component component) {
            p.Setter(p.Value);
            return;
        }

        if (component.Flags.HasFlag(ComponentFlags.Deserializing) || component.Scene.IsEditor) {
            p.Setter(p.Value);
            return;
        }

        GameObject gameObject = component.GameObject;
        if (gameObject.IsValid() && (gameObject.Flags.HasFlag(GameObjectFlags.Deserializing) || gameObject.Flags.HasFlag(GameObjectFlags.Loading))) {
            p.Setter(p.Value);
            return;
        }

        var isFixedUpdate = component.Scene.IsFixedUpdate;
        if (p.GetAttribute<FixedUpdateOnlyAttribute>() is not null && !isFixedUpdate) {
            PropertyDescription propertyDescription = Game.TypeLibrary.GetMemberByIdent(p.MemberIdent) as PropertyDescription;
            Log.Error(new FixedUpdateOnlyException(propertyDescription));
            // throw new FixedUpdateOnlyException( propertyDescription );
        }

        if (p.GetAttribute<UpdateOnlyAttribute>() is not null && isFixedUpdate) {
            PropertyDescription propertyDescription = Game.TypeLibrary.GetMemberByIdent(p.MemberIdent) as PropertyDescription;
            Log.Error(new UpdateOnlyException(propertyDescription));
            // throw new UpdateOnlyException( propertyDescription );
        }

        if (component.Scene.IsFixedUpdate) {
            p.Setter(p.Value);
            return;
        }


        p.Setter(p.Value);
    }
}
