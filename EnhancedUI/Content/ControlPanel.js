var state = '';

async function clearContent() {
    $('#blocks').empty();
}

async function updateContent() {
    const blocks = await model.GetBlocks();
    const ul = $('#blocks');
    ul.empty();
    blocks.forEach(block => {
        let li = $('<li>');
        li.append(document.createTextNode(block.Name));
        ul.append(li);
    });
}