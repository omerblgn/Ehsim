import { useMemo, useState } from 'react';
import MaterialReactTable from 'material-react-table';
import { Box, Button, IconButton, MenuItem, TextField, Tooltip } from '@mui/material';
import { QueryClient, QueryClientProvider, useQuery } from '@tanstack/react-query';
import axios from 'axios';
import { Refresh as RefreshIcon } from '@mui/icons-material';
import { toast } from 'react-toastify';
import { useNavigate } from 'react-router-dom';
import { useEffect } from 'react';
import { Stack } from 'react-bootstrap';
import { style } from '@mui/system';

const Example = () => {
    const sleep = (delay) => new Promise((resolve) => setTimeout(resolve, delay));
    const navigate = useNavigate();

    const [columnFilters, setColumnFilters] = useState([]);
    const [globalFilter, setGlobalFilter] = useState('');
    const [sorting, setSorting] = useState([]);
    const [pagination, setPagination] = useState({
        pageIndex: 0,
        pageSize: 10
    });

    const [urunAdi, setUrunAdi] = useState('');
    const [urunId, setUrunId] = useState('');
    const [teklifDegeri, setTeklifDegeri] = useState('');
    const [musteriId, setMusteriId] = useState('');
    const [openDialog, setOpenDialog] = useState(false);
    const [offer, setOffer] = useState('');
    const [validationErrors, setValidationErrors] = useState({});
    const [eklenenUrunler, setEklenenUrunler] = useState([]);
    const [firstRowSelection, setFirstRowSelection] = useState({});
    const [secondRowSelection, setSecondRowSelection] = useState({});

    const [paraBirimi, setParaBirimi] = useState('TRY');
    const paraBirimleri = [
        {
            value: 'TRY',
            label: '₺'
        },
        {
            value: 'USD',
            label: '$'
        },
        {
            value: 'EUR',
            label: '€'
        }
    ];

    const { data, isError, isFetching, isLoading, refetch } = useQuery({
        queryKey: ['table-data'],
        queryFn: async () => {
            var responseData;
            const FormData = require('form-data');
            let data = new FormData();
            data.append('pageSize', 0);
            data.append('pageIndex', 0);

            let config = {
                method: 'post',
                maxBodyLength: Infinity,
                url: 'http://localhost:5273/api/Urun/GetGrid',
                data: data
            };

            await axios
                .request(config)
                .then((response) => {
                    responseData = response.data.data;
                })
                .catch((error) => {
                    console.log(error);
                });
            return responseData;
        },
        keepPreviousData: true
    });

    const columns = useMemo(
        () => [
            {
                accessorKey: 'adi',
                header: 'Ürün Adı'
            },
            {
                accessorKey: 'aciklama',
                header: 'Açıklama'
            },
            {
                accessorKey: 'ebat',
                header: 'Ebat'
            },
            {
                accessorKey: 'fiyat',
                header: 'Fiyat',
                Cell: ({ cell }) => (
                    <>
                        {cell.getValue()?.toLocaleString?.('tr-TR', {
                            style: 'currency',
                            currency: cell.row.original.paraBirimi,
                            minimumFractionDigits: 0,
                            maximumFractionDigits: 2
                        })}
                    </>
                )
            },
            // {
            //     accessorKey: 'paraBirimi',
            //     header: 'Para Birimi'
            // },
            {
                accessorKey: 'tedarikci',
                header: 'Tedarikçi Firma'
            },
            {
                accessorKey: 'kdv',
                header: 'KDV Oranı (%)'
            },
            {
                accessorKey: 'kategori',
                header: 'Kategori'
            },
            {
                accessorKey: 'urunSahibi',
                header: 'Ürün Sahibi'
            }
        ],
        []
    );

    const calculateFooterValues = () => {
        let toplamTRY = 0;
        let toplamUSD = 0;
        let toplamEUR = 0;

        eklenenUrunler.forEach((urun) => {
            const fiyat = parseFloat(urun.fiyat);
            if (urun.paraBirimi === 'TRY') {
                toplamTRY += fiyat;
            } else if (urun.paraBirimi === 'USD') {
                toplamUSD += fiyat;
            } else if (urun.paraBirimi === 'EUR') {
                toplamEUR += fiyat;
            }
        });

        return {
            toplamTRY,
            toplamUSD,
            toplamEUR
        };
    };

    const selectedColumns = useMemo(
        () => [
            {
                accessorKey: 'adi',
                header: 'Ürün Adı',
                enableEditing: false
            },
            {
                accessorKey: 'fiyat',
                header: 'Fiyat',
                muiTableBodyCellEditTextFieldProps: {
                    required: true,
                    type: 'number',
                    inputProps: {
                        min: 0
                    }
                },
                // aggregationFn: 'sum',
                // AggregatedCell: ({ cell }) => <Box sx={{ color: 'warning.main' }}>{cell.getValue()}</Box>,
                Footer: () => {
                    const { toplamTRY, toplamUSD, toplamEUR } = calculateFooterValues();
                    return (
                        <>
                            <div>Toplam</div>
                            {toplamTRY !== 0 && (
                                <div>
                                    {toplamTRY.toLocaleString('tr-TR', {
                                        style: 'currency',
                                        currency: 'TRY',
                                        minimumFractionDigits: 0,
                                        maximumFractionDigits: 2
                                    })}
                                </div>
                            )}
                            {toplamUSD !== 0 && (
                                <div>
                                    {toplamUSD.toLocaleString('tr-TR', {
                                        style: 'currency',
                                        currency: 'USD',
                                        minimumFractionDigits: 0,
                                        maximumFractionDigits: 2
                                    })}
                                </div>
                            )}
                            {toplamEUR !== 0 && (
                                <div>
                                    {toplamEUR.toLocaleString('tr-TR', {
                                        style: 'currency',
                                        currency: 'EUR',
                                        minimumFractionDigits: 0,
                                        maximumFractionDigits: 2
                                    })}
                                </div>
                            )}
                        </>
                    );
                }
            },
            {
                accessorKey: 'paraBirimi',
                header: 'Para Birimi',
                Edit: ({ cell }) => (
                    <TextField required select defaultValue={cell.row.original.paraBirimi} variant="standard">
                        {paraBirimleri.map((p) => (
                            <MenuItem key={p.value} value={p.value}>
                                {p.label}
                            </MenuItem>
                        ))}
                    </TextField>
                )
            },
            {
                accessorKey: 'adet',
                header: 'Adet',
                muiTableBodyCellEditTextFieldProps: {
                    required: true,
                    type: 'number',
                    defaultValue: 1,
                    inputProps: {
                        min: 0
                    }
                }
            },
            {
                accessorKey: 'kdv',
                header: 'KDV Oranı (%)',
                enableEditing: false
            },
            {
                accessorKey: 'kategori',
                header: 'Kategori',
                enableEditing: false
            },
            {
                accessorKey: 'kdvDahilFiyat',
                header: 'KDV Dahil Fiyat',
                enableEditing: false,
                Cell: ({ cell }) => {
                    const fiyat = parseFloat(cell.row.original.fiyat);
                    const kdv = parseFloat(cell.row.original.kdv);
                    const kdvDahilFiyat = fiyat + (fiyat * kdv) / 100;
                    return (
                        <>
                            {kdvDahilFiyat.toLocaleString('tr-TR', {
                                style: 'currency',
                                currency: cell.row.original.paraBirimi,
                                minimumFractionDigits: 0,
                                maximumFractionDigits: 2
                            })}
                        </>
                    );
                }
                // Footer: 'Toplam: ' + toplam
            }
        ],
        [eklenenUrunler]
    );

    const [expanded, setExpanded] = useState(false);

    const handleChange = (panel) => (event, isExpanded) => {
        setExpanded(isExpanded ? panel : false);
    };

    const handleEkleClick = () => {
        const selectedItems = Object.keys(firstRowSelection);

        // Eklenen ürünleri kontrol et ve sadece yeni olanları eklemek için filtrele
        const newSelectedItems = selectedItems.filter(
            (itemId) => !eklenenUrunler.some((eklenenUrun) => eklenenUrun.id === parseInt(itemId))
        );

        if (newSelectedItems.length === 0) {
            // Uyarı ver: Tüm seçilen ürünler zaten eklenmiş
            alert('Seçili ürünler zaten eklenmiş.');
            return;
        }

        setEklenenUrunler((prevEklenenUrunler) => [
            ...prevEklenenUrunler,
            ...newSelectedItems.map((itemId) => data.list.find((item) => item.id === parseInt(itemId)))
        ]);

        // Seçili öğeleri sıfırla
        setFirstRowSelection({});
    };
    const handleSaveCell = (cell, value) => {
        //if using flat data and simple accessorKeys/ids, you can just do a simple assignment here
        tableData[cell.row.index][cell.column.id] = value;
        //send/receive api updates here
        setTableData([...tableData]); //re-render with new data
    };

    return (
        <>
            <h5>Ürün Ekle</h5>
            <MaterialReactTable
                muiTablePaperProps={{ sx: { marginBottom: '24px' } }}
                enableRowSelection={(row) => !eklenenUrunler.some((eklenenUrun) => eklenenUrun.id === row.id)}
                getRowId={(row) => row.id}
                onRowSelectionChange={setFirstRowSelection}
                muiTableBodyRowProps={({ row }) => ({
                    onClick: row.getToggleSelectedHandler(),
                    sx: { cursor: 'pointer' }
                })}
                columns={columns}
                data={data !== undefined ? data.list : []}
                muiToolbarAlertBannerProps={
                    isError
                        ? {
                              color: 'error',
                              children: 'Error loading data'
                          }
                        : undefined
                }
                onColumnFiltersChange={setColumnFilters}
                onGlobalFilterChange={setGlobalFilter}
                onPaginationChange={setPagination}
                onSortingChange={setSorting}
                renderTopToolbarCustomActions={() => (
                    <Tooltip arrow title="Refresh Data">
                        <IconButton onClick={() => refetch()}>
                            <RefreshIcon />
                        </IconButton>
                    </Tooltip>
                )}
                renderBottomToolbarCustomActions={() => (
                    <Button
                        color="secondary"
                        onClick={handleEkleClick}
                        variant="contained"
                        disabled={Object.keys(firstRowSelection).length === 0}
                    >
                        Ekle
                    </Button>
                )}
                rowCount={data?.dataCount ?? 0}
                state={{
                    columnFilters,
                    globalFilter,
                    isLoading,
                    pagination,
                    showAlertBanner: isError,
                    showProgressBars: isFetching,
                    sorting,
                    rowSelection: firstRowSelection
                }}
            />

            {eklenenUrunler.length > 0 && (
                <>
                    <h5>Eklenen Ürünler</h5>
                    <MaterialReactTable
                        muiTablePaperProps={{ sx: { marginBottom: '24px' } }}
                        enableEditing
                        editingMode="table"
                        muiTableBodyCellEditTextFieldProps={({ cell }) => ({
                            onBlur: (event) => {
                                // handleSaveCell(cell, event.target.value);
                            }
                        })}
                        enableGrouping
                        initialState={{
                            grouping: ['kategori'],
                            expanded: true
                        }}
                        enableRowSelection
                        getRowId={(row) => row.id}
                        onRowSelectionChange={setSecondRowSelection}
                        columns={selectedColumns}
                        data={eklenenUrunler}
                        muiToolbarAlertBannerProps={
                            isError
                                ? {
                                      color: 'error',
                                      children: 'Error loading data'
                                  }
                                : undefined
                        }
                        onColumnFiltersChange={setColumnFilters}
                        onGlobalFilterChange={setGlobalFilter}
                        onPaginationChange={setPagination}
                        onSortingChange={setSorting}
                        renderTopToolbarCustomActions={() => (
                            <Button
                                color="error"
                                onClick={() => {
                                    const selectedIds = Object.keys(secondRowSelection);
                                    const updatedEklenenUrunler = eklenenUrunler.filter(
                                        (item) => !selectedIds.includes(item.id.toString())
                                    );
                                    setEklenenUrunler(updatedEklenenUrunler);
                                    setSecondRowSelection({});
                                }}
                                variant="contained"
                                disabled={Object.keys(secondRowSelection).length === 0}
                            >
                                Sil
                            </Button>
                        )}
                        renderBottomToolbarCustomActions={() => (
                            <Button
                                color="secondary"
                                onClick={() => {
                                    console.log(eklenenUrunler);
                                }}
                                variant="contained"
                            >
                                Kaydet
                            </Button>
                        )}
                        rowCount={data?.dataCount ?? 0}
                        state={{
                            columnFilters,
                            globalFilter,
                            isLoading,
                            pagination,
                            showAlertBanner: isError,
                            showProgressBars: isFetching,
                            sorting,
                            rowSelection: secondRowSelection
                        }}
                    />
                </>
            )}
        </>
    );
};

const queryClient = new QueryClient();

const ExampleWithReactQueryProvider = () => (
    <QueryClientProvider client={queryClient}>
        <Example />
    </QueryClientProvider>
);

export default ExampleWithReactQueryProvider;
