// Custom validation rule: Email with regex (stricter validation)
$.validator.addMethod("customemail", function (value, element) {
    if (!value) return true; // Allow empty (let Required handle it)

    var email = value.trim();

    // Basic checks first
    if (email.startsWith('.'))  return false;
    if (email.startsWith('.') || email.startsWith('@')) return false;
    if (email.endsWith('.') || email.endsWith('@')) return false;
    if (email.includes('..')) return false; // consecutive dots
    if (!email.includes('@')) return false;

    var parts = email.split('@');
    if (parts.length !== 2) return false;
    if (parts[0].length === 0 || parts[1].length === 0) return false;
    if (parts[0].startsWith('.') || parts[0].endsWith('.')) return false;
    if (parts[1].startsWith('.') || parts[1].endsWith('.')) return false;
    if (!parts[1].includes('.')) return false; // domain must have extension

    // RFC 5322 compliant email regex (simplified version)
    var emailRegex = /^[a-zA-Z0-9]([a-zA-Z0-9._-]*[a-zA-Z0-9])?@[a-zA-Z0-9]([a-zA-Z0-9-]*[a-zA-Z0-9])?(\.[a-zA-Z0-9]([a-zA-Z0-9-]*[a-zA-Z0-9])?)+$/;
    return emailRegex.test(email);
}, "Please enter a valid email address");

// Helper function to validate email
function validateEmailInput(email) {
    if (email.startsWith('.'))  return false;
    if (!email || email.length === 0) return null;

    if (email.indexOf(' ') !== -1) {
        return { valid: false, message: '✗ Email cannot contain spaces' };
    }
    if (email.startsWith('.') || email.startsWith('@')) {
        return { valid: false, message: '✗ Email cannot start with . or @' };
    }
    if (email.endsWith('.') || email.endsWith('@')) {
        return { valid: false, message: '✗ Email cannot end with . or @' };
    }
    if (email.includes('..')) {
        return { valid: false, message: '✗ Email cannot have consecutive dots' };
    }
    if (!email.includes('@')) {
        return { valid: false, message: '⚠ Email must contain @ symbol', type: 'warning' };
    }
    if ((email.match(/@/g) || []).length > 1) {
        return { valid: false, message: '✗ Email cannot have multiple @ symbols' };
    }

    var parts = email.split('@');

    if (parts[0].length === 0) {
        return { valid: false, message: '✗ Email must have characters before @' };
    }
    if (parts[0].startsWith('.') || parts[0].endsWith('.')) {
        return { valid: false, message: '✗ Email cannot have dot at start or before @' };
    }
    if (!parts[1] || parts[1].length === 0) {
        return { valid: false, message: '✗ Email must have domain after @' };
    }
    if (!parts[1].includes('.')) {
        return { valid: false, message: '⚠ Email must include domain extension (e.g., @gmail.com)', type: 'warning' };
    }
    if (parts[1].startsWith('.') || parts[1].endsWith('.')) {
        return { valid: false, message: '✗ Domain cannot start or end with dot' };
    }

    // Full regex validation
    var emailRegex = /^[a-zA-Z0-9]([a-zA-Z0-9._-]*[a-zA-Z0-9])?@[a-zA-Z0-9]([a-zA-Z0-9-]*[a-zA-Z0-9])?(\.[a-zA-Z0-9]([a-zA-Z0-9-]*[a-zA-Z0-9])?)+$/;

    if (emailRegex.test(email)) {
        return { valid: true, message: '✓ Valid email address', type: 'success' };
    }

    return { valid: false, message: '✗ Invalid email format' };
}

// Real-time email validation feedback
$('#Email').on('keyup input', function () {
    var email = $(this).val().trim();
    var $field = $(this);

    // Remove previous indicator
    $('.email-indicator').remove();

    if (email.length === 0) {
        $field.removeClass('is-valid is-invalid');
        return;
    }

    var result = validateEmailInput(email);

    if (result) {
        var messageType = result.type || (result.valid ? 'success' : 'danger');
        $field.after('<div class="email-indicator small mt-1 text-' + messageType + '">' + result.message + '</div>');

        // Apply visual feedback immediately
        if (result.valid) {
            $field.removeClass('is-invalid').addClass('is-valid');
        } else {
            $field.removeClass('is-valid').addClass('is-invalid');
        }
    }
});

// On blur, trigger jQuery validation and remove our indicator
$('#Email').on('blur', function () {
    var $field = $(this);

    // Remove our indicator
    $('.email-indicator').remove();

    // Trigger jQuery validation
    $field.valid();
});