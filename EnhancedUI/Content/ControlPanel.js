let displayedVersion = 0;
let rendering = false;

// Invoked from C# whenever a new game state version is available
// noinspection JSUnusedGlobalSymbols
async function OnGameStateChange(version) {
    // Is the model accessible?
    if (TerminalViewModel === undefined)
        return;

    // Eliminate any any duplicate or redundant calls
    if (rendering || version <= displayedVersion)
        return;

    rendering = true;
    try {
        let blockIds = await TerminalViewModel.GetModifiedBlockIds(displayedVersion);
        let blocks = $('#blocks');
        for (const i in blockIds) {
            let blockId = blockIds[i];
            let blockState = await TerminalViewModel.GetBlockState(blockId);
            renderBlock(blocks, blockState);
        }
    } finally {
        displayedVersion = version;
        rendering = false;
    }
}

function renderBlock(parent, blockState) {
    if (blockState == null)
        return;

    let blockViewId = 'block-' + blockState.Id;
    let oldBlockDiv = $('#' + blockViewId);

    let blockDiv = $('<div />');
    blockDiv.addClass('block');
    blockDiv.attr('id', blockViewId);
    renderBlockInner(blockDiv, blockState);

    if (oldBlockDiv.length === 0)
        parent.append(blockDiv);
    else
        oldBlockDiv.replaceWith(blockDiv);
}

function renderBlockInner(blockView, blockState) {
    let blockId = blockState.Id;

    let idDiv = $('<div />');
    idDiv.addClass('id');
    idDiv.text('Block #' + blockId);
    blockView.append(idDiv);

    let typeDiv = $('<div />');
    typeDiv.addClass('type');
    typeDiv.text(blockState.ClassName + ' | ' + blockState.TypeId + ' | ' + blockState.SubtypeName);
    blockView.append(typeDiv);

    let nameDiv = $('<div />');
    nameDiv.addClass('name');
    nameDiv.text(blockState.Name);
    blockView.append(nameDiv);

    let propertiesDiv = $('<div />')
    propertiesDiv.addClass('properties');
    for (const propertyId in blockState.Properties) {
        renderBlockProperty(propertiesDiv, blockState.Id, blockState.Properties[propertyId])
    }
    blockView.append(propertiesDiv)

    blockView.append($('<hr />'))
}

function renderBlockProperty(parent, blockId, propertyState) {
    let propertyId = propertyState.Id;
    let propertyValue = propertyState.Value;

    let propertyDiv= $('<div />');
    let propertyDivId = 'block-' + blockId + '-property-' + propertyId;
    propertyDiv.attr('id', propertyDivId)
    propertyDiv.addClass('property');

    let propertyInputId = 'block-' + blockId + '-property-' + propertyId + '-input'

    let input, label;
    let value = $('<div />');
    value.addClass('value');
    switch(propertyState.TypeName) {
        case "Boolean":
            input = $('<input />')
            input.attr('id', propertyInputId);
            input.attr('type', 'checkbox');
            if (propertyValue) {
                input.attr('checked', 'checked');
            }
            value.append(input);

            label = $('<label />');
            label.attr('for', propertyInputId);
            label.text(propertyId);
            value.append(label);

            value.bind('change', async function (e) {
                await TerminalViewModel.SetBlockProperty(blockId, propertyId, input.is(':checked'));
            });

            break;

        case "Int64":
            label = $('<label />');
            label.attr('for', propertyInputId);
            label.text(propertyId);
            value.append(label);

            input = $('<input />')
            input.attr('id', propertyInputId);
            input.attr('type', 'number');
            input.attr('value', propertyValue == null ? '0' : propertyValue.toString());
            input.attr('maxlength', '20');
            input.attr('size', '20');
            value.append(input);

            value.bind('change', async function (e) {
                await TerminalViewModel.SetBlockProperty(blockId, propertyId, parseInt(input.value()));
            });

            break;

        case "Single":
            label = $('<label />');
            label.attr('for', propertyInputId);
            label.text(propertyId);
            value.append(label);

            input = $('<input />')
            input.attr('id', propertyInputId);
            input.attr('type', 'number');
            input.attr('value', propertyValue == null ? '0.0' : propertyValue.toString());
            input.attr('maxlength', '20');
            input.attr('size', '20');
            value.append(input);

            value.bind('change', async function (e) {
                await TerminalViewModel.SetBlockProperty(blockId, propertyId, parseFloat(input.value()));
            });

            break;

        case "StringBuilder":
            label = $('<label />');
            label.attr('for', propertyInputId);
            label.text(propertyId);
            value.append(label);

            input = $('<input />')
            input.attr('id', propertyInputId);
            input.attr('value', propertyValue == null ? '' : propertyValue.toString());
            input.attr('maxlength', '65535');
            input.attr('size', '100');
            value.append(input);

            value.bind('change', async function (e) {
                await TerminalViewModel.SetBlockProperty(blockId, propertyId, input.value());
            });

            break;

        case "Color":
            // FIXME: Add a color picker on click event!
            // See https://bitbucket.org/chromiumembedded/cef/issues/899

            label = $('<label />');
            label.attr('for', propertyInputId);
            label.text(propertyId);
            value.append(label);

            input = $('<input />')
            input.attr('id', propertyInputId);
            input.attr('type', 'color');
            input.attr('value', propertyValue == null ? '#ffffff' : propertyValue.toString());
            value.append(input);

            value.bind('change', async function (e) {
                await TerminalViewModel.SetBlockProperty(blockId, propertyId, input.value());
            });

            break;

        default:
            label = $('<label />');
            label.attr('for', propertyInputId);
            label.text(propertyId + '[' + propertyState.TypeName + '] ');
            value.append(label);

            input = $('<input />')
            input.attr('readonly', 'readonly');
            input.attr('id', propertyInputId);
            input.attr('value', propertyValue == null ? '' : propertyValue.toString());
            value.append(input);

            break;
    }

    propertyDiv.append(value);

    parent.append(propertyDiv);
}