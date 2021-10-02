async function loadItems() {
    const result = await model.TestAdd(1, 2);
    $('#result').text(result.toString());
}