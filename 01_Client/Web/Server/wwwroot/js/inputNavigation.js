//// inputNavigation.js

//document.addEventListener('keydown', function (event) {
//    const tagName = event.target.tagName.toLowerCase();
//    const interactives = ['input', 'textarea', 'select', 'button', 'a', '[tabindex]'];


//    if (['ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight'].includes(event.key) && interactives.some(tag => event.target.matches(tag))) {       
//        let inputs = Array.from(document.querySelectorAll(interactives.join(',')))
//            .filter(el => el.offsetParent !== null && el.tabIndex >= 0);

//        let currentIndex = inputs.indexOf(event.target);

//        let rows = [];
//        let currentRow = [];
//        let currentTop = inputs[0].getBoundingClientRect().top;
//        inputs.forEach(input => {
//            let top = input.getBoundingClientRect().top;
//            if (top === currentTop) {
//                currentRow.push(input);
//            } else {
//                rows.push(currentRow);
//                currentRow = [input];
//                currentTop = top;
//            }
//        });
//        rows.push(currentRow);
//        let rowIndex = -1, colIndex = -1;
//        rows.forEach((row, rIndex) => {
//            row.forEach((input, cIndex) => {
//                if (input === event.target) {
//                    rowIndex = rIndex;
//                    colIndex = cIndex;
//                }
//            });
//        });

//        event.preventDefault();

//        switch (event.key) {
//            case 'ArrowUp':
//                if (rowIndex - 1 >= 0) {
//                    let targetInput = rows[rowIndex - 1][colIndex] || rows[rowIndex - 1][Math.min(colIndex, rows[rowIndex - 1].length - 1)];
//                    targetInput.focus();
//                }
//                break;
//            case 'ArrowDown':
//                if (rowIndex + 1 < rows.length) {
//                    let targetInput = rows[rowIndex + 1][colIndex] || rows[rowIndex + 1][Math.min(colIndex, rows[rowIndex + 1].length - 1)];
//                    targetInput.focus();
//                }
//                break;
//            case 'ArrowLeft':
//                if (colIndex - 1 >= 0) {
//                    rows[rowIndex][colIndex - 1].focus();
//                } else if (rowIndex - 1 >= 0) {
//                    let targetInput = rows[rowIndex - 1][rows[rowIndex - 1].length - 1];
//                    targetInput.focus();
//                }
//                break;
//            case 'ArrowRight':
//                if (colIndex + 1 < rows[rowIndex].length) {
//                    rows[rowIndex][colIndex + 1].focus();
//                } else if (rowIndex + 1 < rows.length) {
//                    let targetInput = rows[rowIndex + 1][0];
//                    targetInput.focus();
//                }
//                break;
//        }
//    }
//});
