function notTerminated(termDate) {
    var result = false;
    if (termDate === null || new Date(termDate) > new Date()) {
        result = true;
    }
    return result;
}