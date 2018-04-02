function editRule(ruleId){
    var rule = allRules.find(function(item){
        return item.Id == ruleId;
    });
    $.get({
        url:'/api/alerts/linkedActions/'+ruleId,
        success:function(result){
            if(result.Content.length == 0)
            {
                console.log('no linked actions found');
                alert('No linked actions found.');
                return;
            }
            showEditDialog(rule, result.Content);
        },
        error:function(error){
            console.log(error);
            alert('Could not load data, check your network connection.');
        }
    });
}

function showEditDialog(rule, linkedActions){

    currentlyEditedRule = rule;
    currentlyEditedAction = linkedActions[0];
    addNewDialogMode = 'edit';
    $('#addNewRule').modal('show');
    $('#editRuleName').val(rule.Name);
    $('#editRulePattern').val(rule.Pattern);
    $('#actionType').val(linkedActions[0].TypeName);
    onActionTypeValueChanged();
    fillDynamicallyGeneratedFields(linkedActions[0]);
    clearValidationStyles();
}

function fillDynamicallyGeneratedFields(linkedAction){
    let typeDescriptor = actionTypes.find(function(item){
        return item.TypeName == linkedAction.TypeName; 
    });
    typeDescriptor.Properties.forEach(function(prop){
        let elementId = '#ap'+prop.Name;
        $(elementId).val(linkedAction[prop.Name]);
    });
}

function saveChangesToRule(){
    let name = $('#editRuleName').val();
    let pattern = $('#editRulePattern').val();
  
    let selectedActionType = $('#actionType').val();
    var actionToUpdate = getActionToCreate(selectedActionType);
    actionToUpdate.actionId = currentlyEditedAction.Id;

    var updateActionRequest = new Promise(function(resolve, reject){
        $.ajax({
            method:'put',
            url:'/api/alerts',
            data:actionToUpdate,
            dataType:'json',
            success:resolve,
            error:reject
        });
    });

    var updateRuleRequest = new Promise(function(resolve,reject){
        $.ajax({
            url: '/api/blacklist/'+currentlyEditedRule.Id,
            method: 'put',
            data:{name:name, pattern:pattern},
            dataType:'json',
            success: resolve,
            error:reject
        });
    });
    
    updateActionRequest.then(function(actionResult){
        updateRuleRequest.then(function(ruleResult){
            $('#addNewRule').modal('hide');
            loadRules(); 
            currentlyEditedRule = null;
        })
        .catch(function(err){
            showRuleCreation(err);   
        });
    })
    .catch(function(err){
        showRuleCreation(err);   
    });
}

var currentlyEditedRule = null;
var currentlyEditedAction = null;