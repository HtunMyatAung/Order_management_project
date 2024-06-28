function incrementQuantity(itemId) {
    var quantityElement = document.getElementById('quantity-display-' + itemId);
    var quantity = parseInt(quantityElement.innerText);
    quantityElement.innerText = quantity + 1;
}

function decrementQuantity(itemId) {
    var quantityElement = document.getElementById('quantity-display-' + itemId);
    var quantity = parseInt(quantityElement.innerText);
    if (quantity > 1) {
        quantityElement.innerText = quantity - 1;
    }
}

function prepareBuyItem(itemId, itemName, itemPrice) {   
    
    // Update modal with item details
    document.getElementById('modalItemId').innerText = itemId;
    document.getElementById('modalItemName').innerText = itemName;
    document.getElementById('modalItemPrice').innerText = itemPrice;

    var quantity = document.getElementById('quantity-display-' + itemId).innerText;
    document.getElementById('modalItemQuantity').innerText = quantity;

    // Show the modal
    var modal = new bootstrap.Modal(document.getElementById('buyItemModal'));
    modal.show();
}

function submitBuyItem() {
    var itemId = document.getElementById('modalItemId').innerText;
    var quantity = document.getElementById('modalItemQuantity').innerText;

    // Set the hidden form fields with the item ID and quantity
    document.getElementById('selectedItemId').value = itemId;
    document.getElementById('selectedItemQuantity').value = quantity;

    // Submit the form
    document.getElementById('buyItemForm').submit();

    // Close the modal
    var modal = bootstrap.Modal.getInstance(document.getElementById('buyItemModal'));
    modal.hide();
}