document.addEventListener('DOMContentLoaded', function () {
    var nameInput = document.getElementById('nameInput');
    var addressInput = document.getElementById('addressInput');
    var emailInput = document.getElementById('emailInput');
    var phoneInput = document.getElementById('phoneInput');
    var nameValidationMessage = document.getElementById('nameInputValidationMessage');
    var addressValidationMessage = document.getElementById('addressInputValidationMessage');
    var emailValidationMessage = document.getElementById('emailInputValidationMessage');
    var phoneValidationMessage = document.getElementById('phoneInputValidationMessage');
    var passwordInput = document.getElementById('passwordInput');
    var confirmPasswordInput = document.getElementById('confirmPasswordInput');
    var passwordValidationMessage = document.getElementById('passwordInputValidationMessage');
    var confirmPasswordValidationMessage = document.getElementById('confirmPasswordInputValidationMessage');
    var submitButton = document.getElementById('submitButton');

    var emailPattern = /^[a-zA-Z0-9._%+-]+@gmail\.com$/;
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
            submitButton.disabled = true;
        } else {
            validationMessage.innerText = '';
            if (nameInput.value.trim() !== '' && emailPattern.test(emailInput.value) && phonePattern.test(phoneInput.value) && addressInput.value.trim() !== '' && validatePasswordMatch()) {
                submitButton.disabled = false;
            }
        }
    }

    nameInput.addEventListener('input', function () {
        validateInput(nameInput, /.+/, nameValidationMessage); // Accepts any non-empty input for name
    });

    emailInput.addEventListener('input', function () {
        validateInput(emailInput, emailPattern, emailValidationMessage);
    });

    phoneInput.addEventListener('input', function () {
        validateInput(phoneInput, phonePattern, phoneValidationMessage);
    });

    addressInput.addEventListener('input', function () {
        validateInput(addressInput, /.+/, addressValidationMessage); // Accepts any non-empty input for address
    });

    passwordInput.addEventListener('input', validatePasswordMatch);
    confirmPasswordInput.addEventListener('input', validatePasswordMatch);
});