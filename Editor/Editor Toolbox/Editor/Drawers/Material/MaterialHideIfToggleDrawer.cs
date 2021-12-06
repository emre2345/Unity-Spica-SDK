namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Material
{
    public class MaterialHideIfToggleDrawer : MaterialConditionalDrawer
    {
        public MaterialHideIfToggleDrawer(string togglePropertyName) : base(togglePropertyName)
        { }


        protected override bool IsVisible(bool? value)
        {
            return value.HasValue && !value.Value;
        }
    }
}