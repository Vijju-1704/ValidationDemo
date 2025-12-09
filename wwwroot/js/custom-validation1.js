// Custom Client-Side Validation Rules
// File: wwwroot/js/custom-validation.js

$(document).ready(function () {

    // Custom validation rule: No spaces allowed in username
    //$.validator.addMethod("nospaces", function (value, element) {
    //    return this.optional(element) || value.indexOf(' ') === -1;
    //}, "Username cannot contain spaces");

    //// Custom validation rule: Only alphanumeric characters
    //$.validator.addMethod("alphanumeric", function (value, element) {
    //    return this.optional(element) || /^[a-zA-Z0-9]+$/.test(value);
    //}, "Only letters and numbers are allowed");

    // Custom validation rule: Strong password (uppercase, lowercase, number, special char)
    $.validator.addMethod("strongpassword", function (value, element) {
        return this.optional(element) ||
            /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]/.test(value);
    }, "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character");

    // Custom validation rule: Valid Indian phone number
    $.validator.addMethod("indianphone", function (value, element) {
        return this.optional(element) || /^[6-9]\d{9}$/.test(value.replace(/[\s()-]/g, ''));
    }, "Please enter a valid 10-digit Indian mobile number");

    // Custom validation rule: No special characters
    $.validator.addMethod("nospecialchars", function (value, element) {
        return this.optional(element) || /^[a-zA-Z0-9\s]+$/.test(value);
    }, "Special characters are not allowed");

    // Custom validation rule: Minimum age
    $.validator.addMethod("minage", function (value, element, param) {
        var today = new Date();
        var birthDate = new Date(value);
        var age = today.getFullYear() - birthDate.getFullYear();
        var monthDiff = today.getMonth() - birthDate.getMonth();

        if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
            age--;
        }

        return this.optional(element) || age >= param;
    }, "You must be at least {0} years old");

    // Custom validation rule: Valid website URL
    $.validator.addMethod("validwebsite", function (value, element) {
        return this.optional(element) ||
            /^(https?:\/\/)?(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)$/.test(value);
    }, "Please enter a valid website URL");

    // Custom validation rule: Username must start with letter
    $.validator.addMethod("startswithletter", function (value, element) {
        return this.optional(element) || /^[a-zA-Z]/.test(value);
    }, "Username must start with a letter");

    // Custom validation rule: No consecutive special characters
    $.validator.addMethod("noconsecutivespecial", function (value, element) {
        return this.optional(element) || !/[^a-zA-Z0-9]{2,}/.test(value);
    }, "No consecutive special characters allowed");

    // Custom validation rule: Email domain validation (only specific domains)
    $.validator.addMethod("alloweddomains", function (value, element, param) {
        if (this.optional(element)) {
            return true;
        }
        var domain = value.split('@')[1];
        return param.includes(domain);
    }, "Email must be from an allowed domain");

    // Custom validation rule: Password cannot contain username
    $.validator.addMethod("passwordnotusername", function (value, element, param) {
        var username = $(param).val();
        return this.optional(element) || value.toLowerCase().indexOf(username.toLowerCase()) === -1;
    }, "Password cannot contain your username");

    // Custom validation rule: Minimum word count
    $.validator.addMethod("minwords", function (value, element, param) {
        var wordCount = value.trim().split(/\s+/).length;
        return this.optional(element) || wordCount >= param;
    }, "Please enter at least {0} words");

    // Custom validation rule: Maximum word count
    $.validator.addMethod("maxwords", function (value, element, param) {
        var wordCount = value.trim().split(/\s+/).length;
        return this.optional(element) || wordCount <= param;
    }, "Please enter no more than {0} words");

    // Custom validation rule: No numbers allowed
    $.validator.addMethod("nonumbers", function (value, element) {
        return this.optional(element) || !/\d/.test(value);
    }, "Numbers are not allowed");

    // Custom validation rule: Must contain uppercase
    $.validator.addMethod("uppercase", function (value, element) {
        return this.optional(element) || /[A-Z]/.test(value);
    }, "Must contain at least one uppercase letter");

    // Custom validation rule: Must contain lowercase
    $.validator.addMethod("lowercase", function (value, element) {
        return this.optional(element) || /[a-z]/.test(value);
    }, "Must contain at least one lowercase letter");

    // Custom validation rule: Must contain number
    $.validator.addMethod("containsnumber", function (value, element) {
        return this.optional(element) || /\d/.test(value);
    }, "Must contain at least one number");

    // Custom validation rule: Must contain special character
    $.validator.addMethod("specialchar", function (value, element) {
        return this.optional(element) || /[@$!%*?&#]/.test(value);
    }, "Must contain at least one special character (@$!%*?&#)");

    // Custom validation rule: Phone number format (with dashes)
    $.validator.addMethod("phoneformat", function (value, element) {
        return this.optional(element) || /^\d{3}-\d{3}-\d{4}$/.test(value);
    }, "Phone number must be in format: 123-456-7890");

    // Custom validation rule: Postal code validation
    $.validator.addMethod("postalcode", function (value, element) {
        return this.optional(element) || /^\d{6}$/.test(value);
    }, "Please enter a valid 6-digit postal code");

    // Real-time password strength indicator
    $('#Password').on('keyup', function () {
        var password = $(this).val();
        var strength = 0;
        var strengthText = '';
        var strengthClass = '';

        if (password.length >= 6) strength++;
        if (password.length >= 10) strength++;
        if (/[a-z]/.test(password)) strength++;
        if (/[A-Z]/.test(password)) strength++;
        if (/\d/.test(password)) strength++;
        if (/[@$!%*?&#]/.test(password)) strength++;

        switch (strength) {
            case 0:
            case 1:
            case 2:
                strengthText = 'Weak';
                strengthClass = 'text-danger';
                break;
            case 3:
            case 4:
                strengthText = 'Medium';
                strengthClass = 'text-warning';
                break;
            case 5:
            case 6:
                strengthText = 'Strong';
                strengthClass = 'text-success';
                break;
        }

        // Remove existing strength indicator
        $('.password-strength').remove();

        // Add strength indicator
        if (password.length > 0) {
            $('#Password').after('<div class="password-strength small mt-1 ' + strengthClass + '">Password Strength: ' + strengthText + '</div>');
        }
    });

    // Real-time username availability check (example)
    //var usernameTimeout;
    //$('#Username').on('keyup', function () {
    //    clearTimeout(usernameTimeout);
    //    var username = $(this).val();

    //    if (username.length >= 3) {
    //        usernameTimeout = setTimeout(function () {
    //            // Show checking message
    //            $('.username-check').remove();
    //            $('#Username').after('<div class="username-check small mt-1 text-info">Checking availability...</div>');

    //            // In real application, you would make AJAX call to server
    //            // $.ajax({
    //            //     url: '/user/checkusername',
    //            //     data: { username: username },
    //            //     success: function(available) {
    //            //         if(available) {
    //            //             $('.username-check').removeClass('text-danger').addClass('text-success').text('✓ Username available');
    //            //         } else {
    //            //             $('.username-check').removeClass('text-success').addClass('text-danger').text('✗ Username taken');
    //            //         }
    //            //     }
    //            // });

    //            // Simulated check for demo
    //            setTimeout(function () {
    //                $('.username-check').removeClass('text-info').addClass('text-success').text('✓ Username available');
    //            }, 500);
    //        }, 500);
    //    } else {
    //        $('.username-check').remove();
    //    }
    //});

    // Character counter for fields
    function addCharacterCounter(selector, maxLength) {
        $(selector).on('keyup', function () {
            var length = $(this).val().length;
            var remaining = maxLength - length;

            $('.char-counter-' + selector.replace('#', '')).remove();

            var counterClass = remaining < 10 ? 'text-danger' : 'text-muted';
            $(this).after('<div class="char-counter-' + selector.replace('#', '') + ' small mt-1 ' + counterClass + '">' +
                length + ' / ' + maxLength + ' characters</div>');
        });
    }

    // Add character counters (if needed)
    // addCharacterCounter('#Username', 10);
});