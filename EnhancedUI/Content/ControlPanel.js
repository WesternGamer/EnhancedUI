var blockStates = null;

// Invoked from C#
// noinspection JSUnusedGlobalSymbols
async function stateUpdated() {
    const blockViews = $('#blocks');
    blockViews.empty();

    blockStates = await state.GetBlockStates();
    for (const entityId in blockStates) {
        renderBlock(blockViews, blockStates[entityId]);
    }
}

function renderBlock(parent, blockState) {
    let blockView = $('<div />');
    blockView.addClass('block');
    blockView.attr('id', 'block-' + blockState.EntityId);
    renderBlockInner(blockView, blockState);
    parent.append(blockView);
}

function renderBlockInner(blockView, blockState) {
    let id = $('<div />');
    id.addClass('entityId');
    id.text(blockState.EntityId);
    blockView.append(id);

    let type = $('<div />');
    type.addClass('type');
    type.text(blockState.ClassName + ' | ' + blockState.TypeId + ' | ' + blockState.SubtypeName);
    blockView.append(type);

    let name = $('<div />');
    name.addClass('name');
    name.text(blockState.Name);
    blockView.append(name);

    let properties = $('<div />')
    properties.addClass('properties');
    for (const propertyId in blockState.PropertyStates) {
        renderBlockProperty(properties, blockState.PropertyStates[propertyId])
    }
    blockView.append(properties)

    blockView.append($('<hr />'))
}

function renderBlockProperty(parent, propertyState) {
    let propertyView= $('<div />');
    propertyView.addClass('property');

    let value = $('<div />');
    value.addClass('value');
    switch(propertyState.TypeName) {
        case "Boolean":
            cb = $('<input />')
            cb.attr('type', 'checkbox');
            cb.attr('id', propertyState.Id);
            if (propertyState.Value) {
                cb.attr('checked', 'checked');
            }
            value.append(cb);
            label = $('<label />');
            label.attr('for', propertyState.Id);
            label.text(propertyState.Id);
            value.append(label);
            break;
        case "Int64":
            value.text(propertyState.Id + ': ' + propertyState.Value.toString());
            break;
        case "Single":
            value.text(propertyState.Id + ': ' + propertyState.Value.toString());
            break;
        case "Color":
            value.text(propertyState.Id + ': ' + propertyState.Value.toString());
            break;
        default:
            value.text(propertyState.Id + ' ' + propertyState.TypeName + ': ' + (propertyState.Value == null ? 'null' : propertyState.Value.toString()));
    }

    propertyView.append(value);

    parent.append(propertyView);
}

// Invoked from C#
// noinspection JSUnusedGlobalSymbols
async function blockStateUpdated(entityId) {
    // let blockState = await state.GetBlockState(entityId);
    // let blockView = $('#block-' + entityId);
    // if (blockView.length > 0) {
    //     renderBlockInner(blockView, blockState);
    // }
}