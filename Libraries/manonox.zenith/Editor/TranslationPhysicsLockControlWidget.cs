namespace Zenith;

[CustomEditor(typeof(Controller.TranslationPhysicsLock))]
public class TranslationPhysicsLockControlWidget : ControlWidget {
	public Color HighlightColor = Theme.Yellow;

	public override bool SupportsMultiEdit => true;

	public TranslationPhysicsLockControlWidget(SerializedProperty property) : base(property) {
		property.TryGetAsObject(out SerializedObject so);
		MinimumSize = Theme.RowHeight + 2;

		Layout = Layout.Column();
		Layout.Spacing = 2;

		var linear = Layout.AddRow();
		linear.Spacing = 4;

		AddField(so, linear, "X");
		AddField(so, linear, "Y");
		AddField(so, linear, "Z");
		linear.AddStretchCell();

		Cursor = CursorShape.Finger;
	}

	void AddField(SerializedObject so, Layout linear, string propertyName) {
		linear.Add(Create(so.GetProperty(propertyName)));
		linear.Add(new Label(propertyName) { MinimumHeight = Theme.RowHeight, FixedWidth = 25 });
		linear.AddSpacingCell(4);
	}

	protected override void OnPaint() {

	}
}
