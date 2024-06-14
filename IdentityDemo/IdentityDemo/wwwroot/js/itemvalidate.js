document.addEventListener('DOMContentLoaded', function () {
    var nameInput = document.getElementById('nameInput');
    var priceInput = document.getElementById('priceInput');
    var quantityInput = document.getElementById('quantityInput');

    var nameValidationMessage = document.getElementById('nameValidationMessage');
    var priceValidationMessage = document.getElementById('priceValidationMessage');
    var quantityValidationMessage = document.getElementById('quantityValidationMessage');

    var submitButton = document.getElementById('submitButton');

    // Decimal(7,2): Up to 7 digits in total, with up to 2 decimal places.
    var pricePattern = /^\d{1,5}(\.\d{1,2})?$/;
    // Integer pattern for quantity
    var quantityPattern = /^\d+$/;

    function validateInput(input, pattern, validationMessage) {
        if (!pattern.test(input.value.trim())) {
            validationMessage.innerText = input.getAttribute('data-error-message');
            submitButton.disabled = true;
        } else {
            validationMessage.innerText = '';
            checkFormValidity();
        }
    }

    function checkFormValidity() {
        if (
            nameInput.value.trim() !== '' &&
            pricePattern.test(priceInput.value) &&
            quantityPattern.test(quantityInput.value)
        ) {
            submitButton.disabled = false;
        } else {
            submitButton.disabled = true;
        }
    }

    nameInput.addEventListener('input', function () {
        validateInput(nameInput, /.+/, nameValidationMessage);
    });

    priceInput.addEventListener('input', function () {
        validateInput(priceInput, pricePattern, priceValidationMessage);
    });

    quantityInput.addEventListener('input', function () {
        validateInput(quantityInput, quantityPattern, quantityValidationMessage);
    });

    // Initial check on page load
    checkFormValidity();
});