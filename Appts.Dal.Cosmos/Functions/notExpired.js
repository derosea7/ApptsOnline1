function notExpired(expDate) {
    var result = false;
    if (expDate === null || new Date(expDate) > new Date()) {
        result = true;
    }
    return result;
}