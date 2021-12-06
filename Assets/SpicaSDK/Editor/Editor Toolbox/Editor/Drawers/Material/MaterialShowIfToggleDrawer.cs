namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Material
{
    public class MaterialShowIfToggleDrawer : MaterialConditionalDrawer
    {
        public MaterialShowIfToggleDrawer(string togglePropertyName) : base(togglePropertyName)
        { }


        protected override bool IsVisible(bool? value)
        {
            return value ?? false;
        }
    }
}