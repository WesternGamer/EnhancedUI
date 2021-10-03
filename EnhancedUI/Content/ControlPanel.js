async function stateUpdated() {
    const blocks = await state.GetBlocks();
    const ul = $('#blocks');
    ul.empty();
    blocks.forEach(block => {
        let li = $('<li>');
        li.append(document.createTextNode(block.Name));
        ul.append(li);
    });
}