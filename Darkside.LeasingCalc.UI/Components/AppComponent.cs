using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;

namespace fileuploader.ui.Components
{
    /// <summary>
    /// Class AppComponent.
    /// Implements the <see cref="Microsoft.AspNetCore.Components.ComponentBase" />
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Components.ComponentBase" />
    /// <seealso cref="System.IDisposable" />
    public class AppComponent : ComponentBase, IDisposable
    {
        /// <summary>
        /// Gets or sets the authentication state provider.
        /// </summary>
        /// <value>The authentication state provider.</value>
        [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        /// <summary>
        /// Gets or sets the URI helper.
        /// </summary>
        /// <value>The URI helper.</value>
        [Inject] public NavigationManager UriHelper { get; set; }

        /// <summary>
        /// Gets or sets the js runtime instance.
        /// </summary>
        /// <value>The js runtime instance.</value>
        [Inject] public IJSRuntime JSRuntimeInstance { get; set; }

        /// <summary>
        /// Gets or sets the HTTP context accessor.
        /// </summary>
        /// <value>The HTTP context accessor.</value>
        [Inject] public IHttpContextAccessor HttpContextAccessor { get; set; }

        /// <summary>
        /// Gets or sets the state of the authentication.
        /// </summary>
        /// <value>The state of the authentication.</value>
        public AuthenticationState AuthenticationState { get; set; }

        /// <summary>
        /// The list of validation failures 
        /// </summary>
        protected List<ValidationFailure> ValidationFailures { get; private set; }

        /// <summary>
        /// Are there any validation failures?
        /// </summary>
        protected bool HasValidationFailures => ValidationFailures != null && ValidationFailures.Any();

        /// <summary>
        /// on initialized as an asynchronous operation.
        /// </summary>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            AuthenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            ValidationFailures = new List<ValidationFailure>();
        }

        /// <summary>
        /// Get the first validation failure for a specified property name
        /// </summary>
        protected Func<object, string, Task<IEnumerable<string>>> HasValidationFailureForProperty => async (model, propertyName) =>
        {
            return HasValidationFailures ?
                ValidationFailures
                    .Where(item => item.PropertyName.ToLower() == propertyName.ToLower())
                    .Select(item => item.ErrorMessage)
                    .Take(1)
                    .ToList()
                :
                new List<string>();
        };

        /// <summary>
        /// clear the saved list of validation failures
        /// </summary>
        protected void ClearValidationFailures()
        {
            ValidationFailures.Clear();

            StateHasChanged();
        }

        /// <summary>
        /// updates the saved list of validation failures and invokes form validation
        /// </summary>
        protected void SetValidaionFailures(List<ValidationFailure> validationFailures, MudForm mudForm)
        {
            ValidationFailures.Clear();

            ValidationFailures.AddRange(validationFailures);

            mudForm.Validate();

            StateHasChanged();
        }

        public void Dispose()
        {
        }
    }
}
