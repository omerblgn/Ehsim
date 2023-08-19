import React, { useMemo, useState } from 'react';
import MaterialReactTable from 'material-react-table';
import { IconButton, Tooltip, Card, Button, CardContent, Typography, Fade } from '@mui/material';
import RefreshIcon from '@mui/icons-material/Refresh';
import SendIcon from '@mui/icons-material/Send';
import { QueryClient, QueryClientProvider, useQuery } from '@tanstack/react-query';
import axios from 'axios';
import { useEffect } from 'react';
import Box from '@mui/material/Box';
import Modal from '@mui/material/Modal';

const Example = () => {
    const [columnFilters, setColumnFilters] = useState([]);
    const [globalFilter, setGlobalFilter] = useState('');
    const [sorting, setSorting] = useState([]);
    const [pagination, setPagination] = useState({
        pageIndex: 1,
        pageSize: 10
    });

    const { data, isError, isFetching, isLoading, refetch } = useQuery({
        queryKey: ['table-data'],
        //   queryFn: async () => {
        //     const fetchURL = new URL(
        //       '/api/data',
        //       process.env.NODE_ENV === 'production'
        //         ? 'https://www.material-react-table.com'
        //         : 'http://localhost:3000',
        //     );
        //     fetchURL.searchParams.set(
        //       'start',
        //       `${pagination.pageIndex * pagination.pageSize}`,
        //     );
        //     fetchURL.searchParams.set('size', `${pagination.pageSize}`);
        //     fetchURL.searchParams.set('filters', JSON.stringify(columnFilters ?? []));
        //     fetchURL.searchParams.set('globalFilter', globalFilter ?? '');
        //     fetchURL.searchParams.set('sorting', JSON.stringify(sorting ?? []));

        //     const response = await fetch(fetchURL.href);
        //     const json = await response.json();
        //     return json;
        //   },
        //   keepPreviousData: true,

        queryFn: async () => {
            var responseData;
            const FormData = require('form-data');
            let data = new FormData();
            data.append('pageSize', 0);
            data.append('pageIndex', 0);

            let config = {
                method: 'post',
                maxBodyLength: Infinity,
                url: 'https://localhost:5273/Customer/GetCustomerGrid',
                data: data
            };

            await axios
                .request(config)
                .then((response) => {
                    console.log(JSON.stringify(response.data));
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
                header: 'İsim'
            },
            {
                accessorKey: 'soyadi',
                header: 'Soyisim'
            },
            {
                accessorKey: 'telefonNumarasi',
                header: 'Telefon Numarası'
            },
            {
                accessorKey: 'email',
                header: 'Email Adresi'
            }
        ],
        []
    );

    return (
        <>
            <MaterialReactTable
                columns={columns}
                data={data !== undefined ? data.list : []} //data is undefined on first render
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
                rowCount={data?.dataCount ?? 0}
                state={{
                    columnFilters,
                    globalFilter,
                    isLoading,
                    pagination,
                    showAlertBanner: isError,
                    showProgressBars: isFetching,
                    sorting
                }}
            />
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
