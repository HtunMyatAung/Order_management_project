document.addEventListener('DOMContentLoaded', function () {
    var nameInput = document.getElementById('nameInput');
    var addressInput = document.getElementById('addressInput');
    var emailInput = document.getElementById('emailInput');
    var phoneInput = document.getElementById('phoneInput');
    var nameValidationMessage = document.getElementById('nameValidationMessage');
    var addressValidationMessage = document.getElementById('addressValidationMessage');
    var emailValidationMessage = document.getElementById('emailValidationMessage');
    var phoneValidationMessage = document.getElementById('phoneValidationMessage');
    var submitButton = document.getElementById('submitButton');

    var emailPattern = /^[a-zA-Z0-9._%+-]+@gmail\.com$/;
    var phonePattern = /^09\d{9}$/; // Simple pattern for a 10-digit phone number

    function validateInput(input, pattern, validationMessage) {
        if (!pattern.test(input.value)) {
            validationMessage.innerText = input.getAttribute('data-error-message');
            submitButton.disabled = true;
        } else {
            validationMessage.innerText = '';
            checkFormValid();
        }
    }

    function checkFormValid() {
        if (
            nameInput.value.trim() !== '' &&
            emailPattern.test(emailInput.value) &&
            phonePattern.test(phoneInput.value) &&
            addressInput.value.trim() !== ''
        ) {
            submitButton.disabled = false;
        } else {
            submitButton.disabled = true;
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

    checkFormValid();
});