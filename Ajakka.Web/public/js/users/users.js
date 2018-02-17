function loadEndpoints(){
    var currentPage = $('#currentPage').text();
    if(!currentPage){
        currentPage = 0;
    }

    $.get({
        url: './api/users/'+currentPage+'/10',
        success: fillTableWithUsers,
        error:showError
      });
    $.get({
        url:'./api/users/pageCount?pageSize=10',
        success:showPageCount
    })
} 

function showPageCount(pageCountResponse){
    var pageCount = pageCountResponse.pageCount;
    var currentPage = $('#currentPage').text();
    if(!currentPage){
        currentPage = 0;
    }
    
    if(currentPage > 0){
        var previousPage = currentPage - 1;
        $('#pageCount').append('<a href="./index?page='+previousPage+'">&lt;&lt; ');    
    }
    currentPage++;
    $('#pageCount').append(' ' + currentPage + '/' + pageCount);

    if(currentPage < pageCount){
        var nextPage = currentPage;
        $('#pageCount').append('<a href="./index?page='+nextPage+'"> &gt;&gt; ');    
    }
}

function showError(error){
    console.log(error);
    $('#userListContainer').empty();
    $('#userListContainer').append('<tr class="table-danger"><td colspan="4">Request error: '+error.statusText+'</td></tr>');
}

function fillTableWithUsers(users){
    $('#userListContainer').empty();
    
    users.forEach(function(user){
      
        var row = '<tr>';
        row += '<td>' + user.name + '</td>';
        row += '<td><a href="">delete</a></td>';
        row += '</tr>';
        $('#userListContainer').append(row);
        
    });
}

setTimeout(loadEndpoints, 1000);

