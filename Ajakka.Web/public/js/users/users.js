function loadUsers(){
    var currentPage = $('#currentPage').text();
    if(!currentPage){
        currentPage = 0;
    }

    $.get({
        url: '/api/users/'+currentPage+'/10',
        success: fillTableWithUsers,
        error:showError
      });
    $.get({
        url:'/api/users/pageCount?pageSize=10',
        success:showPageCount
    })
} 

function showPageCount(pageCountResponse){
    $('#pageCount').text('');
    var pageCount = pageCountResponse.pageCount;
    var currentPage = $('#currentPage').text();
    if(!currentPage){
        currentPage = 0;
    }
    
    if(currentPage > 0){
        var previousPage = currentPage - 1;
        $('#pageCount').append('<a href="/settings/users?page='+previousPage+'"><i class="fas fa-caret-left"> ');    
    }
    currentPage++;
    $('#pageCount').append(' ' + currentPage + '/' + pageCount);

    if(currentPage < pageCount){
        var nextPage = currentPage;
        $('#pageCount').append('<a href="/settings/users?page='+nextPage+'"> <i class="fas fa-caret-right"> ');    
    }
}

function showError(error){
    console.log(error);
    $('#userListContainer').empty();
    $('#userListContainer').append('<tr class="table-danger"><td colspan="4">Request error: '+error.statusText+'</td></tr>');
}

function fillTableWithUsers(users){
    $('#userListContainer').empty();
    var currentUser = $('#currentUserName').text();

    users.forEach(function(user){
        let disabled = '';
        let tooltip = 'Delete this user';
        if(currentUser == user.name){
            disabled = ' disabled';
            tooltip = 'You cannot delete your own account';
        }
        var row = '<tr>';
        row += '<td>' + user.name + '</td>';
        row += '<td style="text-align:right"><button class="btn btn-secondary" href="#" onclick="deleteUser(\''+user.id+'\',\''+user.name+'\')"'+disabled+'><i class="fas fa-trash-alt"/><span data-toggle="tooltip" title="'+tooltip+'"> delete</span></a></td>';
        row += '</tr>';
        $('#userListContainer').append(row);
        
    });
    $('[data-toggle="tooltip"]').tooltip()
}

function deleteUser(id, name){
    $('.tooltip').hide();
    var currentUser = $('#currentUserName').text();
    if(currentUser == name){
        alert('You cannot delete your own account.');
        return;
    }
    $.get({
        method:'DELETE',
        url: '/api/users/'+id,
        success: loadUsers,
        error:showError
      });
}

function toggleAddUserForm(){
    $('#addNewUser').toggle();
    $('#addNewButton').toggle();
}

function addNewUser(){
    $('#newUserName').removeClass('is-invalid');
    $('#password').removeClass('is-invalid');
    $('#passwordRepeat').removeClass('is-invalid');
    $('#addNewUserError').hide();

    var name = $('#newUserName').val();
    if(name == '' || name == null){
        $('#newUserName').addClass('is-invalid');
        return false;
    }
    var password = $('#password').val();
    if(password == '' || password == null){
        $('#password').addClass('is-invalid');
        return false;
    }
    var passwordRepeat = $('#passwordRepeat').val();
    if(password != passwordRepeat){
        $('#passwordRepeat').addClass('is-invalid');
        return false;
    }
    
    var post = new Promise(function(resolve, reject){
        $.post({
            url: '/api/users/',
            data:{name:name, pwd:password},
            dataType:'json',
            success: resolve,
            error:reject
        });
    });
    post.then(function(result){
        $('#addNewUser').hide();
        $('#addNewButton').show();  
        loadUsers(); 
    }).catch(function(error){
        showUserCreationError(error);
        
    });
}

function showUserCreationError(error){
    console.log(error);
    $('#addNewUserError').text(error.responseText);
    $('#addNewUserError').show();
}

setTimeout(loadUsers, 100);