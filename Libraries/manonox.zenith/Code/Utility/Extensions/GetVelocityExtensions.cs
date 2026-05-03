namespace Zenith.Utility.Extensions;


public static class GetVelocityExtensions {
    public static Vector3 GetVelocity(this Collider collider) {
        var localPoint = collider.LocalBounds.Center;
        if (collider.Rigidbody.IsValid()) {
            localPoint = collider.Rigidbody.OverrideMassCenter ? collider.Rigidbody.MassCenterOverride : collider.Rigidbody.MassCenter;
        }

        var worldPoint = collider.WorldTransform.PointToWorld(localPoint);
        return collider.GetVelocityAtPoint(worldPoint);
    }

    public static Vector3 GetVelocity(this Rigidbody rigidbody) {
        var localPoint = rigidbody.OverrideMassCenter ? rigidbody.MassCenterOverride : rigidbody.MassCenter;
        var worldPoint = rigidbody.WorldTransform.PointToWorld(localPoint);
        return rigidbody.GetVelocityAtPoint(worldPoint);
    }
}
