document.addEventListener('DOMContentLoaded', function () {
    var form = document.getElementById('changePasswordForm');
    var oldPassword = document.getElementById('oldPassword');
    var newPassword = document.getElementById('newPassword');
    var confirmPassword = document.getElementById('confirmPassword');
    var oldPasswordError = document.getElementById('oldPasswordError');
    var newPasswordError = document.getElementById('newPasswordError');
    var confirmPasswordError = document.getElementById('confirmPasswordError');
    var submitButton = document.getElementById('submitButton');

    function validateOldPassword() {
        if (oldPassword.value.trim() === '') {
            oldPasswordError.innerText = 'Old password is required';
            return false;
        } else {
            oldPasswordError.innerText = '';
            return true;
        }
    }

    function validateNewPassword() {
        if (newPassword.value.trim().length < 6) {
            newPasswordError.innerText = 'New password must be at least 6 characters';
            return false;
        } else {
            newPasswordError.innerText = '';
            return true;
        }
    }

    function validateConfirmPassword() {
        if (confirmPassword.value.trim() !== newPassword.value.trim()) {
            confirmPasswordError.innerText = 'Passwords do not match';
            return false;
        } else {
            confirmPasswordError.innerText = '';
            return true;
        }
    }

    oldPassword.addEventListener('input', validateOldPassword);
    newPassword.addEventListener('input', validateNewPassword);
    confirmPassword.addEventListener('input', validateConfirmPassword);

    form.addEventListener('submit', function (event) {
        var isOldPasswordValid = validateOldPassword();
        var isNewPasswordValid = validateNewPassword();
        var isConfirmPasswordValid = validateConfirmPassword();

        if (!isOldPasswordValid || !isNewPasswordValid || !isConfirmPasswordValid) {
            event.preventDefault(); // Prevent form submission if validation fails
        }
    });
});
