// Auto-dismiss alerts after 5 seconds
setTimeout(function () {
    document.querySelectorAll('.alert-dismissible').forEach(function (el) {
        var bsAlert = bootstrap.Alert.getOrCreateInstance(el);
        bsAlert.close();
    });
}, 5000);
