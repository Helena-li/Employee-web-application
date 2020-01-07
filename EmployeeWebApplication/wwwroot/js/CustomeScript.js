function confirmDelete(uniqueId, isDelete) {
    var deletespan = "ClickDelete_" + uniqueId;
    var confirmDelete = "ConfirmDelete_" + uniqueId;

    if (isDelete) {
        $('#' + deletespan).hide();
        $('#' + confirmDelete).show();
    } else {
        $('#' + deletespan).show();
        $('#' + confirmDelete).hide();
    }
}