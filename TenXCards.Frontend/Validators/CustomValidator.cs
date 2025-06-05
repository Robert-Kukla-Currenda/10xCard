using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;

namespace TenXCards.Frontend.Validators
{
    public class CustomValidation : ComponentBase
    {
        private ValidationMessageStore? messageStore;

        [CascadingParameter]
        private EditContext? CurrentEditContext { get; set; }

        protected override void OnInitialized()
        {
            if (CurrentEditContext is null)
            {
                throw new InvalidOperationException(
                    $"{nameof(CustomValidation)} requires a cascading " +
                    $"parameter of type {nameof(EditContext)}.");
            }

            messageStore = new(CurrentEditContext);

            CurrentEditContext.OnValidationRequested += (s, e) =>
                messageStore?.Clear();
            CurrentEditContext.OnFieldChanged += (s, e) =>
                messageStore?.Clear(e.FieldIdentifier);
        }

        public void DisplayErrors(Dictionary<string, List<string>> errors)
        {
            if (CurrentEditContext is null) return;

            foreach (var err in errors)
            {
                messageStore?.Add(CurrentEditContext.Field(err.Key), err.Value);
            }

            CurrentEditContext.NotifyValidationStateChanged();
        }

        public void ClearErrors()
        {
            messageStore?.Clear();
            CurrentEditContext?.NotifyValidationStateChanged();
        }
    }
}
