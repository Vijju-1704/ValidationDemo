// wwwroot/js/alphanumeric-validator.js

(function ($) {
    // Add custom validation method
    $.validator.addMethod("alphanumeric", function (value, element, params) {
        // Allow empty values (let [Required] handle that)
        if (!value || value.trim() === '') {
            return true;
        }

        // Check if value contains only letters and numbers
        var alphanumericRegex = /^[a-zA-Z0-9]+$/;
        return alphanumericRegex.test(value);
    });
    // Custom validation rule: Valid Indian phone number
    $.validator.addMethod("indianphone", function (value, element) {
        return this.optional(element) || /^[6-9]\d{9}$/.test(value.replace(/[\s()-]/g, ''));
    }, "Please enter a valid 10-digit Indian mobile number");

    // Register the unobtrusive adapter
    $.validator.unobtrusive.adapters.add("alphanumeric", [], function (options) {
        options.rules["alphanumeric"] = true;
        options.messages["alphanumeric"] = options.message;
    });
})(jQuery);
