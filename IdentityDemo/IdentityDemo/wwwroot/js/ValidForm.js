document.addEventListener('DOMContentLoaded', function () {
    var nameInput = document.getElementById('nameInput');
    var addressInput = document.getElementById('addressInput');
    var emailInput = document.getElementById('emailInput');
    var phoneInput = document.getElementById('phoneInput');
    var passwordInput = document.getElementById('passwordInput');
    var confirmPasswordInput = document.getElementById('confirmPasswordInput');
    var nameValidationMessage = document.getElementById('nameInputValidationMessage');
    
    var emailValidationMessage = document.getElementById('emailInputValidationMessage');
    var phoneValidationMessage = document.getElementById('phoneInputValidationMessage');
    var passwordValidationMessage = document.getElementById('passwordInputValidationMessage');
    var confirmPasswordValidationMessage = document.getElementById('confirmPasswordInputValidationMessage');
    var submitButton = document.getElementById('submitButton');

    var emailPattern = /^[a-zA-Z0-9._%+-]+@gmail\.com$/;
    var passwordPattern = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[\W_]).{6,}$/;
    var phonePattern = /^09\d{9}$/; // Simple pattern for a 10-digit phone number

    function validatePasswordMatch() {
        if (passwordInput.value !== confirmPasswordInput.value) {
            confirmPasswordValidationMessage.innerText = 'Passwords do not match';
            return false;
        } else {
            confirmPasswordValidationMessage.innerText = '';
            return true;
        }
    }

    function validateInput(input, pattern, validationMessage) {
        if (!pattern.test(input.value)) {
            validationMessage.innerText = input.getAttribute('data-error-message');
            return false;
        } else {
            validationMessage.innerText = '';
            return true;
        }
    }

    function validateForm() {
        var isFormValid =
            validateInput(nameInput, /.+/, nameValidationMessage) &&
            validateInput(emailInput, emailPattern, emailValidationMessage) &&
            validateInput(phoneInput, phonePattern, phoneValidationMessage) &&            
            validateInput(passwordInput, passwordPattern, passwordValidationMessage) &&
            validatePasswordMatch();

        submitButton.disabled = !isFormValid;
    }

    nameInput.addEventListener('input', function () {
        validateInput(nameInput, /.+/, nameValidationMessage);
        validateForm();
    });

    emailInput.addEventListener('input', function () {
        validateInput(emailInput, emailPattern, emailValidationMessage);
        validateForm();
    });

    phoneInput.addEventListener('input', function () {
        validateInput(phoneInput, phonePattern, phoneValidationMessage);
        validateForm();
    });
    
    passwordInput.addEventListener('input', function () {
        validateInput(passwordInput, passwordPattern, passwordValidationMessage);
        validateForm();
    });

    confirmPasswordInput.addEventListener('input', function () {
        validatePasswordMatch();
        validateForm();
    });

    validateForm(); // Initial validation
});
