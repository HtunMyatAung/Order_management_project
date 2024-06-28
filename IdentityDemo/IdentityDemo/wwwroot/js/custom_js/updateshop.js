document.addEventListener('DOMContentLoaded', function () {
    var nameInput = document.getElementById('nameInput');
    var phoneInput = document.getElementById('phoneInput');
    var emailInput = document.getElementById('emailInput');
    var addressInput = document.getElementById('addressInput');

    var nameValidationMessage = document.getElementById('nameValidationMessage');
    var phoneValidationMessage = document.getElementById('phoneValidationMessage');
    var emailValidationMessage = document.getElementById('emailValidationMessage');
    var addressValidationMessage = document.getElementById('addressValidationMessage');

    var submitButton = document.querySelector('button[type="submit"]');

    var emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    var phonePattern = /^09\d{9}$/; 
    function validateInput(input, pattern, validationMessage) {
        if (!pattern.test(input.value.trim())) {
            validationMessage.innerText = input.getAttribute('data-error-message');
        } else {
            validationMessage.innerText = '';
        }
        checkFormValidity();
    }

    function checkFormValidity() {
        if (
            nameInput.value.trim() !== '' &&
            phonePattern.test(phoneInput.value) &&
            emailPattern.test(emailInput.value) &&
            addressInput.value.trim() !== ''
        ) {
            submitButton.disabled = false;
        } else {
            submitButton.disabled = true;
        }
    }

    nameInput.addEventListener('input', function () {
        validateInput(nameInput, /.+/, nameValidationMessage);
    });

    phoneInput.addEventListener('input', function () {
        validateInput(phoneInput, phonePattern, phoneValidationMessage);
    });

    emailInput.addEventListener('input', function () {
        validateInput(emailInput, emailPattern, emailValidationMessage);
    });

    addressInput.addEventListener('input', function () {
        validateInput(addressInput, /.+/, addressValidationMessage);
    });

    // Initial check on page load
    checkFormValidity();
});
