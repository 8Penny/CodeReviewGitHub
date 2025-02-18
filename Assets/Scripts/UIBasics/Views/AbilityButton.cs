namespace UIBasics.Views
{
    public class AbilityButton: BaseClickButton
    {
        protected override void OnClick()
        {
            _uiService.Views.AbilityWindowView.OnResearchButtonClicked();
        }
    }
}