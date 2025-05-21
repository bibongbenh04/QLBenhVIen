document.addEventListener('DOMContentLoaded', () => {
    const searchInputs = document.querySelectorAll('[data-search-table]');

    searchInputs.forEach(input => {
        const tableId = input.getAttribute('data-search-table');
        const colIndexes = input.getAttribute('data-search-cols')?.split(',').map(c => parseInt(c.trim())) || [0];
        const tableRows = document.querySelectorAll(`#${tableId} tbody tr`);

        input.addEventListener('input', () => {
            const keyword = input.value.toLowerCase();

            tableRows.forEach(row => {
                const isMatch = colIndexes.some(index => {
                    const cell = row.querySelector(`td:nth-child(${index + 1})`);
                    const text = cell?.textContent?.toLowerCase() || '';
                    return text.includes(keyword);
                });

                row.style.display = isMatch ? '' : 'none';
            });
        });
    });
});